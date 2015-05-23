using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace IntouchAfrica2.DataAccess
{
    [TableName("AttendanceSet")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class AttendanceSet
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }  //The date that the Attendance Set is for

        [Column("CapturedByMemberId")]
        public int CapturedByMemberId { get; set; }

        [Column("CaptureTime")]
        public DateTime CaptureTime { get; set; }  //Audit time when this set was captured. 

        [Column("StructuralGroupId")]
        public int StructuralGroupId { get; set; }

        [ResultColumn]
        public IEnumerable<AttendanceRecord> AttendanceRecords { get; set; }
    }
}