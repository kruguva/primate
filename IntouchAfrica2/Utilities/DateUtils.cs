using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.Utilities
{
    public class DateUtils
    {
        public static IEnumerable<DateTime> Range(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate) throw new ArgumentException("End date must be after start date");
            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                yield return currentDate;
                currentDate = currentDate.AddDays(1);
            }
        }
    }
}