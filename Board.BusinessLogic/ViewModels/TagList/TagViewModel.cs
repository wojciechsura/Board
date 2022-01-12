using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.Models.Data;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.TagList
{
    public class TagViewModel : BaseViewModel
    {
        private readonly TagModel tag;
        private readonly ITagHandler handler;

        private void DoDelete()
        {
            handler.RequestDelete(this);
        }

        private void DoEdit()
        {
            handler.RequestEdit(this);
        }

        public TagViewModel(TagModel tag, ITagHandler handler)
        {
            this.tag = tag;
            this.handler = handler;

            EditCommand = new AppCommand(obj => DoEdit());
            DeleteCommand = new AppCommand(obj => DoDelete());
        }

        public string Name => tag.Name;
        public int Color => tag.Color;

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public TagModel Tag => tag;
    }
}
