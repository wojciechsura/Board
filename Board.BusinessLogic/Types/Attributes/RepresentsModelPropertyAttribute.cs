using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RepresentsModelPropertyAttribute : Attribute
    {
        public RepresentsModelPropertyAttribute(string modelProperty)
        {
            ModelProperty = modelProperty;
        }

        public string ModelProperty { get; }
    }
}
