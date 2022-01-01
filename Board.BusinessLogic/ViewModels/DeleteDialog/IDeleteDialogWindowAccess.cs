using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.DeleteDialog
{
    public interface IDeleteDialogWindowAccess
    {
        void Close(bool result);
    }
}
