using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace IntouchAfrica2.DataAccess
{
    [TableName("AbsenteeNote")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class AbsenteeNote
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("CapturedByMemberId")]
        public int CapturedByMemberId { get; set; }

        [ResultColumn]
        public Member CapturedBy {get;set;}

        [Column("Reason")]
        public string Reason { get; set; }

        [Column("DateFrom")]
        public DateTime DateFrom { get; set; }

        [Column("DateTo")]
        public DateTime DateTo { get; set; }

    }
}