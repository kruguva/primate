using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace IntouchAfrica2.DataAccess
{
    [TableName("GroupMember")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class GroupMember
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("StructuralGroupId")]
        public int StructuralGroupId { get; set; }

        [Column("MemberId")]
        public int MemberId { get; set; }

        [ResultColumn]
        public Member Member { get; set; }

        [ResultColumn]
        public StructuralGroup Group { get; set; }
    }
}