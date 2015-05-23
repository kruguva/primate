using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace IntouchAfrica2.DataAccess
{
    public enum StructuralGroupType { 
        Leaf = 1, 
        Intermediate = 2, 
        Root = 3
    }

    [TableName("StructuralGroup")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class StructuralGroup
    {
        public StructuralGroup()
        {
            ChildGroups = new List<StructuralGroup>();
            Members = new List<Member>();
        }

        public StructuralGroup(StructuralGroupType type, string name) : this()
        {
            Type = (int)type;
            Name = name;
        }

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Type")]
        public int Type { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("ParentId")]
        public int ParentId { get; set; }

        [ResultColumn]
        public StructuralGroup Parent { get; set; }

        [ResultColumn]
        public List<Member> Members { get; set; }

        [ResultColumn]
        public List<StructuralGroup> ChildGroups { get; set; }

        [ResultColumn]
        public string TypeDescription
        {
            get { return ((StructuralGroupType)Type).ToString(); }
        }
    }
}