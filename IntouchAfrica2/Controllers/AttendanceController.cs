using IntouchAfrica2.DataAccess;
using IntouchAfrica2.Utilities;
using IntouchAfrica2.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace IntouchAfrica2.Controllers
{
    public class AttendanceController : UmbracoApiController
    {
        [HttpPost]
        [MemberAuthorize(AllowType="Teacher")]
        public AttendanceSetViewModel PostAttendanceSet(AttendanceSet attendanceSet)
        {
            var db = ApplicationContext.DatabaseContext.Database;
            ValidateAttendanceSetOrThrow(attendanceSet, db);

            attendanceSet.CaptureTime = DateTime.Now;
            var currentMember = Members.GetCurrentMember();
            attendanceSet.CapturedByMemberId = currentMember == null ? 0 : currentMember.Id;
            attendanceSet.Date = attendanceSet.Date.Date;

            db.Insert(attendanceSet);
            foreach (var attendanceRecord in attendanceSet.AttendanceRecords)
            {
                attendanceRecord.AttendanceSetId = attendanceSet.Id;
                db.Insert(attendanceRecord);
            }

            return GetAttendanceSet(attendanceSet.StructuralGroupId, attendanceSet.Date);
        }

        [MemberAuthorize(AllowType = "Teacher")]
        public AttendanceSetViewModel GetAttendanceSet(int structuralGroupId, DateTime date)
        {
            var db = ApplicationContext.DatabaseContext.Database;
            var attendanceSet = db.FirstOrDefault<AttendanceSet>("SELECT * FROM AttendanceSet WHERE StructuralGroupId = @0 AND [Date] = @1", structuralGroupId, date.Date);
            if (attendanceSet == null) return null;
            var attendanceSetViewModel = AttendanceSetViewModel.FromModel(attendanceSet);

            if (attendanceSet.CapturedByMemberId != 0)
            {
                var captureMember = (IPublishedContent)Members.GetById(attendanceSet.CapturedByMemberId);
                attendanceSetViewModel.CapturedByMemberName = captureMember.Name;
            }

            var attendanceRecords = db.Fetch<AttendanceRecord>("SELECT * FROM AttendanceRecord WHERE AttendanceSetId = @0", attendanceSet.Id);
            attendanceSetViewModel.AttendanceRecords = attendanceRecords;

            return attendanceSetViewModel;
        }

        [MemberAuthorize(AllowType = "Student,Parent")]
        public IEnumerable<StudentAttendanceSummaryViewModel> GetAttendanceForStudents(DateTime start, DateTime end)
        {
            var currentMember = Members.GetCurrentMember();
            var member = Services.MemberService.GetById(currentMember.Id);
            if (currentMember.ContentType.Alias == "Parent")
            {
                var children = GetChildrenForParent(member.Id);
                return children.Select(child => GetAttendanceSummaryForStudent(child, start, end));
            }
            else
            {
                return new[] { GetAttendanceSummaryForStudent(member, start, end) };
            }
        }

        private IEnumerable<IMember> GetChildrenForParent(int parentId)
        {
            var children1 = Services.MemberService.GetMembersByPropertyValue("parent1", parentId);
            var children2 = Services.MemberService.GetMembersByPropertyValue("parent2", parentId);
            return children1.Concat(children2);
        }

        private StudentAttendanceSummaryViewModel GetAttendanceSummaryForStudent(IMember student, DateTime start, DateTime end)
        {
            var db = ApplicationContext.DatabaseContext.Database;
            var query = string.Format("SELECT [Date], Attendance FROM AttendanceRecord INNER JOIN AttendanceSet ON AttendanceRecord.AttendanceSetId = AttendanceSet.Id WHERE MemberID = {0}", student.Id);
            var table = DbUtils.ExecuteDataTable(db, query);

            var summaryVm = new StudentAttendanceSummaryViewModel()
            {
                StudentId = student.Id,
                Name = student.Name,
                AttendanceSummaries = DateUtils.Range(start, end).Select(date =>
                    new AttendanceSummaryViewModel
                    {
                        Date = date,
                        Present = table.AsEnumerable().Any(r => r.Field<DateTime>("Date") == date.Date && r.Field<int>("Attendance") == (int)Attendance.Present) ? 1 : 0,
                        Absent = table.AsEnumerable().Any(r => r.Field<DateTime>("Date") == date.Date && r.Field<int>("Attendance") == (int)Attendance.Absent) ? 1 : 0
                    }
                )
            };

            return summaryVm;
        }

        [MemberAuthorize(AllowType = "Student,Parent")]
        public IEnumerable<AbsenteeViewModel> GetAbsenteeism(DateTime start, DateTime end) {
            var currentMember = Members.GetCurrentMember();
            var member = Services.MemberService.GetById(currentMember.Id);
            if (currentMember.ContentType.Alias == "Parent")
            {
                var children = GetChildrenForParent(member.Id);
                return children.Select(child => GetAbsenteeismForStudent(child, start, end));
            }
            else
            {
                return new[] { GetAbsenteeismForStudent(member, start, end) };
            }
        }

        private AbsenteeViewModel GetAbsenteeismForStudent(IMember student, DateTime start, DateTime end)
        {
            var db = ApplicationContext.DatabaseContext.Database;

            var query1 = string.Format("SELECT [Date] FROM AttendanceRecord " +
                            "INNER JOIN AttendanceSet ON AttendanceRecord.AttendanceSetId = AttendanceSet.Id " +
                            "WHERE MemberId = {0} " +
                            "AND Attendance = {1} " +
                            "AND [Date] BETWEEN '{2}' AND '{3}'", student.Id, (int)Attendance.Absent, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));

            var query2 = string.Format("SELECT COUNT(*) FROM AttendanceRecord " +
                            "INNER JOIN AttendanceSet ON AttendanceRecord.AttendanceSetId = AttendanceSet.Id " +
                            "WHERE MemberId = {0} " +
                            "AND [Date] BETWEEN '{1}' AND '{2}'", student.Id, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));

            var absentTable = DbUtils.ExecuteDataTable(db, query1);
            var count = db.ExecuteScalar<int>(query2);

            return new AbsenteeViewModel()
            {
                StudentId = student.Id,
                Name = student.Name,
                StartDate = start,
                EndDate = end,
                AbsentDates = absentTable.AsEnumerable().Select(r => r.Field<DateTime>("Date")),
                TotalDays = count
            };
        }

        [MemberAuthorize(AllowType = "Teacher")]
        public IEnumerable<AttendanceSummaryViewModel> GetAttendanceSummary(DateTime start, DateTime end, int structuralGroupId)  
        {
            var db = ApplicationContext.DatabaseContext.Database;
            using (var structureRepository = new StructureRepository(db, Services.MemberService))
            {
                var structure = structureRepository.GetStructureFrom(structuralGroupId);
                var ids = new List<int>();
                PopulateStructureIds(structure, ids);

                var query = string.Format("SELECT [Date], Attendance, COUNT(*) AS Total FROM AttendanceRecord " +
                                "INNER JOIN AttendanceSet ON AttendanceRecord.AttendanceSetId = AttendanceSet.Id " +
                                "WHERE [Date] BETWEEN '{0}' and '{1}' " +
                                "AND StructuralGroupId IN ({2}) " +
                                "GROUP BY [Date], Attendance", start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"), string.Join(",", ids));

                var table = DbUtils.ExecuteDataTable(db, query);
                return DateUtils.Range(start, end).Select(date =>
                {
                    var presentRow = table.Select(string.Format("Date = '{0:yyyy-MM-dd}' AND Attendance = {1}", date, (int)Attendance.Present)).FirstOrDefault();
                    var absentRow = table.Select(string.Format("Date = '{0:yyyy-MM-dd}' AND Attendance = {1}", date, (int)Attendance.Absent)).FirstOrDefault();
                    var present = presentRow == null ? 0 : presentRow.Field<int>("Total");
                    var absent = absentRow == null ? 0 : absentRow.Field<int>("Total");
                    return new AttendanceSummaryViewModel
                    {
                        Date = date,
                        Present = present,
                        Absent = absent
                    };
                });
            }
        }

        private void PopulateStructureIds(StructuralGroup root, List<int> ids)
        {
            ids.Add(root.Id);
            if (root.ChildGroups.Any())
            {
                foreach (var child in root.ChildGroups)
                    PopulateStructureIds(child, ids);
            }
        }

        private void ValidateAttendanceSetOrThrow(AttendanceSet attendanceSet, UmbracoDatabase db)
        {
            if (attendanceSet.StructuralGroupId <= 0) throw new Exception("Invalid group specified");
            var requiredMembers = MemberHelper.GetMembers(ApplicationContext.DatabaseContext.Database, Services.MemberService, attendanceSet.StructuralGroupId, "Student");

            if (attendanceSet.AttendanceRecords.Select(a=>a.MemberId).Except(requiredMembers.Select(m=>m.Id)).Any() ||
                requiredMembers.Select(m => m.Id).Except(attendanceSet.AttendanceRecords.Select(a => a.MemberId)).Any()) 
                throw new Exception("Attendance has not been captured for all members of the group");

            if (!attendanceSet.AttendanceRecords.All(a => a.Attendance == (int)Attendance.Absent || a.Attendance == (int)Attendance.Present))
                throw new Exception("Invalid present/absent status");
        }
    }
}
