using Board.Models.Data;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class AddedTagViewModel : BaseTagViewModel
    {
        public AddedTagViewModel(TagModel tag, IEntryEditorHandler handler) 
            : base(tag, handler)
        {

        }
    }
}
