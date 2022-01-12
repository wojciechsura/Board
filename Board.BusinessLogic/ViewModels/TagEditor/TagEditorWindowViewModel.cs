using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.Models.Data;
using Board.Resources;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.TagEditor
{
    public class TagEditorWindowViewModel : BaseViewModel
    {
        private readonly TagModel tag;
        private readonly ITagEditorWindowAccess access;

        [SyncWithModel(nameof(TagModel.Name))]
        private string name;
        [SyncWithModel(nameof(TagModel.Color))]
        private int color;

        private void DoCancel()
        {
            access.Close(false);
        }

        private void DoOk()
        {
            UpdateToModel(tag);
            access.Close(true);
        }

        public TagEditorWindowViewModel(TagModel tag, ITagEditorWindowAccess access, bool isNew)
        {
            this.tag = tag;
            this.access = access;

            if (isNew)
                Title = Strings.TagEditor_Title_New;
            else
                Title = Strings.TagEditor_Title_Edit;

            OkCommand = new AppCommand(obj => DoOk());
            CancelCommand = new AppCommand(obj => DoCancel());

            UpdateFromModel(tag);
        }

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public int Color
        {
            get => color;
            set => Set(ref color, value);
        }

        public string Title { get; }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public TagModel Result => tag;
    }
}
