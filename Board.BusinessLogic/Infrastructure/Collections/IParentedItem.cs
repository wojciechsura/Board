using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Infrastructure.Collections
{
    public interface IParentedItem<TParent>
    {
        TParent Parent { get; set; }
    }
}
