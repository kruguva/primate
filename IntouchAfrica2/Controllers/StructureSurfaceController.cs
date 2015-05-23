using IntouchAfrica2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace IntouchAfrica2.Controllers
{
    public class StructureSurfaceController : SurfaceController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ClearData()
        {
            try
            {
                //AttendanceRecord, AttendanceSet, GroupMember, StructuralGroup
                var db = ApplicationContext.DatabaseContext.Database;
                db.Execute("DELETE FROM AttendanceRecord");
                db.Execute("DELETE FROM AttendanceSet");
                db.Execute("DELETE FROM GroupMember");
                db.Execute("DELETE FROM StructuralGroup");

                var parents = Services.MemberService.GetMembersByMemberType("Parent");
                if (parents.Any())
                    Services.MemberService.DeleteMembersOfType(parents.First().ContentType.Id);

                var students = Services.MemberService.GetMembersByMemberType("Student");
                if (students.Any())
                    Services.MemberService.DeleteMembersOfType(students.First().ContentType.Id);

                var teachers = Services.MemberService.GetMembersByMemberType("Teacher");
                if (teachers.Any())
                    Services.MemberService.DeleteMembersOfType(teachers.First().ContentType.Id);

                TempData.Add("Success", "Cleared all data");
            }
            catch (Exception e)
            {
                TempData.Add("Error", e.Message);
            }
            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public ActionResult UploadStructure(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                try
                {
                    var table = StreamToTable(file.InputStream);
                    CreateStructureFromTable(table);
                    TempData.Add("Success", "Successfully created structure from file");
                }
                catch (Exception e)
                {
                    TempData.Add("Error", "Error while processing structure: " + e.Message);
                }
            }
            else
            {
                TempData.Add("Error", "Empty File");
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public ActionResult UploadMembers(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                try
                {
                    var table = StreamToTable(file.InputStream);
                    CreateMembersFromTable(table);
                    TempData.Add("Success", "Successfully created members from file");
                }
                catch (Exception e)
                {
                    TempData.Add("Error", "Error while processing structure: " + e.Message);
                }
            }
            else
            {
                TempData.Add("Error", "Empty File");
            }

            return RedirectToCurrentUmbracoPage();
        }

        private DataTable StreamToTable(Stream stream)
        {
            var reader = new StreamReader(stream);
            string line = null;
            DataTable table = new DataTable();
            bool first = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (first)
                {
                    table.Columns.AddRange(line.Split(',').Select(s => new DataColumn(s.Trim())).ToArray());
                    first = false;
                }
                else
                {
                    var row = table.NewRow();
                    row.ItemArray = line.Split(',').Select(s => s.Trim()).ToArray();
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        [HttpPost]
        public ActionResult CreateDummyData()
        {
            try
            {
                var rootGroup = CreateStructure(); //Members already created, save structure
                var db = ApplicationContext.DatabaseContext.Database;
                db.Insert(rootGroup);
                Console.WriteLine("Inserted root Group with id : " + rootGroup.Id);

                foreach (var grade in rootGroup.ChildGroups)
                {
                    grade.ParentId = rootGroup.Id;
                    db.Insert(grade);
                    Console.WriteLine("Inserted Grade Group with id : " + grade.Id);
                    foreach (var @class in grade.ChildGroups)
                    {
                        @class.ParentId = grade.Id;
                        db.Insert(@class);
                        Console.WriteLine("Inserted Class Group with id : " + @class.Id);
                        foreach (var student in @class.Members)
                        {
                            var groupMember = new GroupMember()
                            {
                                MemberId = student.Id,
                                StructuralGroupId = @class.Id
                            };
                            db.Insert(groupMember);
                        }
                    }
                }

                TempData.Add("Success", "Successfully created dummy data");
            }
            catch (Exception e)
            {
                TempData.Add("Error", "An error occured while creating the dummy data: " + e.Message);
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public ActionResult AddToGroup()
        {
            try
            {
                int totalRecords;
                var members = ApplicationContext.Services.MemberService.GetAll(0, 10000, out totalRecords);
                foreach (var member in members)
                {
                    ApplicationContext.Services.MemberService.AssignRole(member.Id, "AllUsers");
                }
                TempData.Add("Success", "Successfully assigned groups");
            }
            catch (Exception e)
            {
                TempData.Add("Error", "An error occured while assigning groups: " + e.Message);
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public ActionResult CreateTeachers()
        {
            try
            {
                var teacherType = ApplicationContext.Services.MemberTypeService.Get("Teacher");
                ApplicationContext.Services.MemberService.DeleteMembersOfType(teacherType.Id);

                var root = GetRoot();
                //Create principal
                var principal = CreateTeacher("Principal");
                AddGroupMember(principal.Id, root.Id);

                foreach (var grade in root.ChildGroups)
                {
                    //Create HOD
                    var hod = CreateTeacher(grade.Name);
                    AddGroupMember(hod.Id, grade.Id);

                    foreach (var @class in grade.ChildGroups)
                    {
                        var teacher = CreateTeacher(@class.Name);
                        AddGroupMember(teacher.Id, @class.Id);
                    }
                }
                TempData.Add("Success", "Successfully created teachers");
            }
            catch (Exception e)
            {
                TempData.Add("Error", "An error occured while creating teachers: " + e.Message);
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public ActionResult CreateParents()
        {
            try
            {
                var parents = Services.MemberService.GetMembersByMemberType("Parent");
                if (parents.Any())
                    Services.MemberService.DeleteMembersOfType(parents.First().ContentType.Id);

                var students = Services.MemberService.GetMembersByMemberType("Student");
                foreach (var grade in Enumerable.Range(1, 7))
                {
                    foreach (var @class in Enumerable.Range(65, 4).Select(i => ((char)i).ToString()))
                    {
                        foreach (var studentNumber in Enumerable.Range(1, 5))
                        {
                            if (studentNumber == 5 && grade % 2 == 0) continue;
                            var name = "Parent" + grade + @class + studentNumber;
                            var parent = CreateParent(name);
                            SetParent(string.Format("Student{0}{1}{2}@joesprimary.nosend.net", grade, @class, studentNumber), parent, students);

                            if (studentNumber == 5 && grade % 2 == 1 && grade < 7)
                                SetParent(string.Format("Student{0}{1}{2}@joesprimary.nosend.net", grade + 1, @class, studentNumber), parent, students);
                        }
                    }
                }

                TempData.Add("Success", "Successfully created parents");
            }
            catch (Exception e)
            {
                TempData.Add("Error", "An error occured creating parents: " + e.Message);
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public ActionResult Test()
        {
            var students = Services.MemberService.GetMembersByMemberType("Student");
            var student = students.Single(s => s.Username == "Student1A1@joesprimary.nosend.net");
            var val = student.GetValue("parent");
            TempData.Add("Success", val.ToString());
            return RedirectToCurrentUmbracoPage();
        }

        private void SetParent(string studentUsername, Member parent, IEnumerable<IMember> students)
        {
            var student = students.Single(s => s.Username == studentUsername);
            student.SetValue("parent", parent.Id);
            Services.MemberService.Save(student);
        }

        private void AddGroupMember(int memberId, int structuralGroupId)
        {
            var groupMember = new GroupMember()
            {
                MemberId = memberId,
                StructuralGroupId = structuralGroupId
            };
            ApplicationContext.DatabaseContext.Database.Insert(groupMember);
        }

        private StructuralGroup CreateStructure()
        {
            var root = new StructuralGroup(StructuralGroupType.Root, "Joe's Primary");
            foreach (var gradeNumber in Enumerable.Range(1, 7))
            {
                var grade = new StructuralGroup(StructuralGroupType.Intermediate, "Grade " + gradeNumber);
                foreach (var classLetter in Enumerable.Range(65, 4).Select(i => ((char)i).ToString()))
                {
                    var className = string.Format("{0}{1}", gradeNumber, classLetter);
                    var @class = new StructuralGroup(StructuralGroupType.Leaf, className);
                    @class.Parent = grade;
                    grade.ChildGroups.Add(@class);

                    foreach (var studentNumber in Enumerable.Range(1, 5))
                    {
                        var member = CreateStudent(className, studentNumber);
                        @class.Members.Add(member);
                    }
                }
                grade.Parent = root;
                root.ChildGroups.Add(grade);
            }

            return root;
        }



        private Member CreateTeacher(string className)
        {
            var email = string.Format("Teacher{0}@joesprimary.nosend.net", className.Replace(" ", ""));
            var name = string.Format("Teacher {0}", className);
            return CreateMember("Teacher", name, email);
        }

        private Member CreateStudent(string className, int number)
        {
            var email = string.Format("Student{0}{1}@joesprimary.nosend.net", className, number);
            var name = string.Format("Student {0} {1}", className, number);
            return CreateMember("Student", name, email);
        }

        private Member CreateParent(string name)
        {
            var email = name + "@joesprimary.nosend.net";
            return CreateMember("Parent", name, email);
        }

        private Member CreateMember(string type, string name, string email)
        {
            var member = (Member)ApplicationContext.Services.MemberService.CreateMember(email, email, name, type);
            Services.MemberService.Save(member);
            ApplicationContext.Services.MemberService.SavePassword(member, name.Replace(" ", ""));
            ApplicationContext.Services.MemberService.AssignRole(member.Id, "AllUsers");
            return member;
        }

        public StructuralGroup GetRoot()
        {
            var db = ApplicationContext.DatabaseContext.Database;
            var allGroups = db.Fetch<StructuralGroup>("SELECT * FROM StructuralGroup");

            var root = allGroups.Single(g => g.Type == (int)StructuralGroupType.Root);
            PopulateChildren(root, allGroups);

            return root;
        }

        private void PopulateChildren(StructuralGroup group, List<StructuralGroup> allGroups)
        {
            group.ChildGroups = allGroups.Where(g => g.ParentId == group.Id).ToList();
            foreach (var child in group.ChildGroups)
                PopulateChildren(child, allGroups);
        }

        private void CreateStructureFromTable(DataTable table)
        {
            VerifyTableOrThrow(table, new[] { "Name", "Parent" });
            var db = ApplicationContext.DatabaseContext.Database;
            foreach (var row in table.AsEnumerable())
            {
                var parent = row.Field<string>("Parent");
                var isLeaf = int.Parse(row.Field<string>("IsLeaf").Trim());
                var type = isLeaf == 1 ? StructuralGroupType.Leaf : string.IsNullOrWhiteSpace(parent) ? StructuralGroupType.Root : StructuralGroupType.Intermediate;
                var group = new StructuralGroup(type, row.Field<string>("Name"));

                if (!string.IsNullOrWhiteSpace(parent))
                {
                    var p = GetStructuralGroupByName(parent);
                    group.Parent = p;
                    group.ParentId = p.Id;
                }
                db.Insert(group);
            }
        }

        private StructuralGroup GetStructuralGroupByName(string name)
        {
            var db = ApplicationContext.DatabaseContext.Database;
            var results = db.Fetch<StructuralGroup>("SELECT * FROM StructuralGroup WHERE Name = '" + name.Replace("'", "''") + "'");
            if (!results.Any()) throw new Exception("There is no structural group with name: " + name);
            return results[0];
        }

        private void CreateMembersFromTable(DataTable table)
        {
            VerifyTableOrThrow(table, new[] { "Type", "Name", "Email", "StructureName", "ParentEmail1", "ParentEmail2" });
            //Create a parent1 and parent2 property on the student member type and update attendance to use it accordingly.

            foreach (var row in table.AsEnumerable())
            {
                var member = CreateMember(row.Field<string>("Type"), row.Field<string>("Name"), row.Field<string>("Email"));
                if (row.Field<string>("Type") == "Student")
                {
                    if (!string.IsNullOrWhiteSpace(row.Field<string>("ParentEmail1")))
                        SetParentProperty(member, "parent1", row.Field<string>("ParentEmail1"));

                    if (!string.IsNullOrWhiteSpace(row.Field<string>("ParentEmail2")))
                        SetParentProperty(member, "parent2", row.Field<string>("ParentEmail2"));
                }

                if (!string.IsNullOrWhiteSpace(row.Field<string>("StructureName")))
                {
                    var structure = GetStructuralGroupByName(row.Field<string>("StructureName"));
                    AddGroupMember(member.Id, structure.Id);
                }
            }
        }

        private void SetParentProperty(IMember member, string property, string parentEmail)
        {
            var parents = Services.MemberService.GetMembersByMemberType("Parent");
            var parent = parents.FirstOrDefault(p => p.Username == parentEmail);
            if (parent == null) throw new Exception("Could not find parent with email " + parentEmail);
            member.SetValue("parent1", parent.Id);
            Services.MemberService.Save(member);
        }

        private void VerifyTableOrThrow(DataTable table, string[] requiredColumns)
        {
            if (requiredColumns.Any(s => !table.Columns.Contains(s)))
                throw new Exception("Table does not contain the required columns: " + string.Join(", ", requiredColumns));

        }
    }
}