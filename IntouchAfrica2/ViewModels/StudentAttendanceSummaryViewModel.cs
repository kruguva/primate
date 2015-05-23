using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.ViewModels
{
    public class StudentAttendanceSummaryViewModel
    {
        public string Name { get; set; }
        public int StudentId { get; set; }
        public IEnumerable<AttendanceSummaryViewModel> AttendanceSummaries { get; set; }
    }
}