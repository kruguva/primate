using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace IntouchAfrica2.DataAccess
{
    public enum Attendance
    {
        Present = 1, Absent = 2
    };

    [TableName("AttendanceRecord")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class AttendanceRecord
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("MemberId")]
        public int MemberId { get; set; }

        [Column("Attendance")]
        public int Attendance { get; set; }

        [Column("AttendanceSetId")]
        public int AttendanceSetId { get; set; }

        [ResultColumn]
        public AttendanceSet AttendanceSet { get; set; }
    }
}