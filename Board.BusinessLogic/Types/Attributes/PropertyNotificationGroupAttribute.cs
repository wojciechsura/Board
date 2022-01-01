using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Types.Attributes
{
    public class PropertyNotificationGroupAttribute : Attribute
    {
        public PropertyNotificationGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public string GroupName { get; }
    }
}
