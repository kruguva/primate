using IntouchAfrica2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.ViewModels
{
    public class StructuralGroupViewModel
    {
        public int Id { get; set; }
        public string TypeDescription { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string ParentGroupName { get; set; }
        public int ParentId { get; set; }

        public static StructuralGroupViewModel FromModel(StructuralGroup model)
        {
            return new StructuralGroupViewModel {
                Id = model.Id,
                Type = model.Type,
                Name = model.Name,
                ParentId = model.ParentId,
                ParentGroupName = model.Parent == null ? string.Empty : model.Parent.Name,
                TypeDescription = Enum.GetName(typeof(StructuralGroupType), model.Type)
            };
        }

        public bool ValidateNew(StructuralGroup root)
        {
            return !string.IsNullOrEmpty(Name) && (ParentId > 0 || root == null);
        }

        public bool ValidateUpdate(StructuralGroup root)
        {
            return ValidateNew(root) && Id != null;
        }
    }
}