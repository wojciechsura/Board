using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Types.Enums
{
    [Flags]
    public enum EditActions
    {
        Add = 1,
        Edit = 2,
        Delete = 4
    }
}
