using IntouchAfrica2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;

namespace IntouchAfrica2.ViewModels
{
    public class AttendanceSummaryViewModel
    {
        public DateTime Date { get; set; }

        public int Total
        {
            get
            {
                return Present + Absent;
            }
        }

        public int Present { get; set; }
        public int Absent { get; set; }
    }
}