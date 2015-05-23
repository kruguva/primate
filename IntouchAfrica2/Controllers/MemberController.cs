using IntouchAfrica2.DataAccess;
using IntouchAfrica2.Events;
using IntouchAfrica2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.WebApi;

namespace IntouchAfrica2.Controllers
{
    public class SchoolmembershipController : UmbracoApiController
    {
        private static List<PropertyViewModel> _standardProperties = new List<PropertyViewModel>
        {
            PropertyViewModel.Mandatory("Email", "Email Address", "Umbraco.Textbox"),
            PropertyViewModel.Mandatory("Name", "Name", "Umbraco.Textbox"),
            PropertyViewModel.Mandatory("Password", "Password", "Umbraco.Password")
        };

        [HttpPost]
        public void Logout()
        {
            if (Members.IsLoggedIn())
                FormsAuthentication.SignOut();
        }

        [HttpGet]
        public MemberCreationViewModel CreationModel(string type)
        {
            var memberType = Services.MemberTypeService.Get(type);
            if (memberType == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var propertyGroups = memberType.PropertyGroups;
            var typeProperties = memberType.CompositionPropertyTypes
                    .Except(propertyGroups.SelectMany(p=>p.PropertyTypes))  //Remove anything that is not a "Generic Property"
                    .Where(p=>p.PropertyEditorAlias != "Umbraco.NoEdit")
                    .OrderBy(p=>p.SortOrder)
                    .Select(p => new PropertyViewModel()
                        {
                            Name = p.Alias,
                            Prompt = p.Name,
                            Type = p.PropertyEditorAlias,
                            IsMandatory = p.Mandatory,
                            AvailableValues = Services.DataTypeService.GetPreValuesByDataTypeId(p.DataTypeDefinitionId),
                            Information = p.HelpText
                        }
                    );

            var viewModel = new MemberCreationViewModel()
            {
                Type = type,
                Properties = _standardProperties.Concat(typeProperties)
            };
            
            return viewModel;
        }

        [HttpPost]
        public MemberViewModel CreateFromModel(MemberCreationViewModel model)
        {
            if (!model.Validate() || _standardProperties.Except(model.Properties, new PropertyNameComparer()).Any() || string.IsNullOrEmpty(model.Type))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var name = model.Properties.Single(p=>p.Name == "Name").Value;
            var email = model.Properties.Single(p=>p.Name == "Email").Value;
            var password = model.Properties.Single(p=>p.Name == "Password").Value;

            var member = (Member) Services.MemberService.CreateMember(email, email, name, model.Type);
            foreach(var property in model.Properties.Except(_standardProperties, new PropertyNameComparer()))
                member.SetPropertyValue(property.Name, property.Value);

            Services.MemberService.Save(member);
            Services.MemberService.SavePassword(member, password);
            Services.MemberService.AssignRole(member.Id, "AllUsers");

            EventAggregator.Instance.Publish(new MemberCreated(member));

            return new MemberViewModel(member);
        }

        [HttpGet]
        public IEnumerable<MemberViewModel> GetMembers(string type)
        {
            var members = Services.MemberService.GetMembersByMemberType(type);
            return members.Select(m=>new MemberViewModel(m));
        }

        [HttpGet]
        public Dictionary<string, object> GetMemberInfo(int id)
        {
            var member = Services.MemberService.GetById(id);
            if (member == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var memberDictionary = new Dictionary<string, object>();
            memberDictionary.Add("memberType", member.ContentTypeAlias);
            foreach (var prop in member.Properties.Where(p=>!p.Alias.StartsWith("umbracoMember") || p.Alias == "umbracoMemberName"))
                memberDictionary.Add(prop.Alias, prop.Value);

            return memberDictionary;
        }

        [HttpPost]
        public GroupMemberViewModel AddMemberToGroup(GroupMemberViewModel groupMember)
        {
            var member = Services.MemberService.GetById(groupMember.MemberId);
            var structuralGroup = ApplicationContext.DatabaseContext.Database.Query<StructuralGroup>("SELECT * FROM StructuralGroup WHERE Id = " + groupMember.StructuralGroupId).FirstOrDefault();  //TODO: Move this to a common domain service

            if (member == null || structuralGroup == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            MemberHelper.AddMemberToGroup(ApplicationContext.DatabaseContext.Database, groupMember.MemberId, groupMember.StructuralGroupId);

            return new GroupMemberViewModel(new MemberViewModel(member), StructuralGroupViewModel.FromModel(structuralGroup));
        }

        [HttpDelete]
        public void RemoveMemberFromGroup(int memberId, int structuralGroupId)
        {
            using (var repository = new GroupMemberRepository(ApplicationContext.DatabaseContext.Database))
            {
                var groupMember = repository.Get(memberId, structuralGroupId);
                if (groupMember == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                repository.Delete(groupMember);
            }
        }
    }
}