using IntouchAfrica2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntouchAfrica2.Controllers
{
    public class PropertyNameComparer : IEqualityComparer<PropertyViewModel>
    {
        public bool Equals(PropertyViewModel x, PropertyViewModel y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(PropertyViewModel obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
