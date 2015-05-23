using IntouchAfrica2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.ViewModels
{
    public class AttendanceSetViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CapturedByMemberId { get; set; }
        public string CapturedByMemberName { get; set; }
        public DateTime CaptureTime { get; set; }
        public int StructuralGroupId { get; set; }

        public IEnumerable<AttendanceRecord> AttendanceRecords { get; set; }

        public static AttendanceSetViewModel FromModel(AttendanceSet model)
        {
            return new AttendanceSetViewModel()
            {
                Id = model.Id,
                Date = model.Date,
                CapturedByMemberId = model.CapturedByMemberId,
                CaptureTime = model.CaptureTime,
                StructuralGroupId = model.StructuralGroupId
            };
        }
    }
}