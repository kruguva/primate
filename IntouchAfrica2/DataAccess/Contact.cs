using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace IntouchAfrica2.DataAccess
{
    public enum ContactType  //Change to an entity, implementing class can have behaviour on how to interact with the communication channel. 
    {
        [Description("Email")]
        Email = 1,
        [Description("Sms")]
        Sms = 2, 
        [Description("What's App")]
        WhatsApp = 3
    }

    [TableName("Contact")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class Contact
    {
        [Column("ContactType")]
        public int ContactType { get; set; }

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Address")]
        public string Address { get; set; }

        [Column("MemberId")]
        public int MemberId { get; set; }
    }
}