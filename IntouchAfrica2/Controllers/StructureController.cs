using IntouchAfrica2.DataAccess;
using IntouchAfrica2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace IntouchAfrica2.Controllers
{
    public class StructureController : UmbracoApiController
    {
        public StructuralGroup GetRoot()  //TODO: Make this a ViewModel
        {
            using (var structureRepository = new StructureRepository(ApplicationContext.DatabaseContext.Database, ApplicationContext.Services.MemberService))
            {
                return structureRepository.GetRootForMember(Members.GetCurrentMember());
            }
        }

        [MemberAuthorize(AllowType="Teacher")]
        public IEnumerable<StructuralGroupViewModel> GetLeaves()
        {
            using (var structureRepository = new StructureRepository(ApplicationContext.DatabaseContext.Database, ApplicationContext.Services.MemberService))
            {
                var root = structureRepository.GetRootForMember(Members.GetCurrentMember());
                var leaves = new List<StructuralGroup>();
                structureRepository.PopulateLeaves(root, leaves);
                var viewModels = leaves.Select(StructuralGroupViewModel.FromModel);
                return viewModels;
            }
        }

        public IEnumerable<MemberViewModel> GetMembers(int structuralGroupId, string memberType)
        {
            using (var structureRepository = new StructureRepository(ApplicationContext.DatabaseContext.Database, ApplicationContext.Services.MemberService))
            {
                return structureRepository.GetMembers(structuralGroupId, memberType).Select(MemberViewModel.FromModel);
            }
        }

        [HttpPost]
        public StructuralGroupViewModel InsertGroup(StructuralGroupViewModel viewModel)
        {
            var root = GetRoot();

            if (!viewModel.ValidateNew(root))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var type = root == null ? StructuralGroupType.Root 
                : viewModel.TypeDescription == "Class" ? StructuralGroupType.Leaf : StructuralGroupType.Intermediate;

            var model = new StructuralGroup(type, viewModel.Name);
            model.ParentId = viewModel.ParentId;

            using (var structureRepository = new StructureRepository(ApplicationContext.DatabaseContext.Database, ApplicationContext.Services.MemberService))
            {
                structureRepository.Insert(model);
            }

            return StructuralGroupViewModel.FromModel(model);
        }

        [HttpPut]
        public StructuralGroupViewModel UpdateGroup(StructuralGroupViewModel viewModel)
        {
            var root = GetRoot();

            if (!viewModel.ValidateUpdate(root))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            using (var structureRepository = new StructureRepository(ApplicationContext.DatabaseContext.Database, ApplicationContext.Services.MemberService))
            {
                var targetModel = structureRepository.GetById(viewModel.Id);
                targetModel.Name = viewModel.Name;
                targetModel.ParentId = viewModel.ParentId;
                structureRepository.Update(targetModel);
            }

            return viewModel;
        }

        [HttpDelete]
        public void DeleteGroup(int id)
        {
            using (var structureRepository = new StructureRepository(ApplicationContext.DatabaseContext.Database, ApplicationContext.Services.MemberService))
            {
                var model = structureRepository.GetById(id);
                if (model == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                structureRepository.Delete(model);
            }
        }



    }
}