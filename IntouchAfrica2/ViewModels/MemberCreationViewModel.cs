using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.ViewModels
{
    public class MemberCreationViewModel
    {
        public string Type { get;set;}
        public IEnumerable<PropertyViewModel> Properties { get; set; }

        public bool Validate()
        {
            return Properties.All(p => !p.IsMandatory || !string.IsNullOrEmpty(p.Value));
        }
    }

    public class PropertyViewModel
    {
        public string Name { get; set; }
        public string Prompt { get; set; }
        public string Information { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public IEnumerable<string> AvailableValues { get; set; }
        public bool IsMandatory { get; set; }

        public static PropertyViewModel Mandatory(string name, string prompt, string type) {
            return new PropertyViewModel() {
                Name = name,
                Prompt = prompt,
                Type = type, 
                IsMandatory = true
            };
        }
    }
}