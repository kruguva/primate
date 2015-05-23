using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace IntouchAfrica2.ViewModels
{
    public class MemberViewModel
    {
        public MemberViewModel()
        {

        }

        public MemberViewModel(IMember model)
        {
            Id = model.Id;
            Name = model.Name;
            var photoProperty = model.Properties.SingleOrDefault(p=>p.Alias == "Photo");
            Photo = photoProperty == null ? null : photoProperty.Value.ToString();
            Type = model.ContentType.Alias;
            Email = model.Email;
            LastLoginDate = model.LastLoginDate;
            Username = model.Username;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }  //TODO: Make this a binary stream
        public string Type { get; set; }
        public string Email { get; set; }
        public DateTime LastLoginDate { get; set;}
        public string Username { get; set; }

        public static MemberViewModel FromModel(IMember model)
        {
            return new MemberViewModel(model);
        }
    }
}