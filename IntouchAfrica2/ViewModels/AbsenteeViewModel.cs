using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.ViewModels
{
    public class AbsenteeViewModel
    {
        public string Name { get; set; }
        public int StudentId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<DateTime> AbsentDates { get; set; }

        public int TotalDays { get; set; }

        //Can add name of the student to work for parents with multiple students 
    }
}