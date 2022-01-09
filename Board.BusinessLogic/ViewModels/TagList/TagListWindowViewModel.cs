using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.Types.Enums;
using Board.BusinessLogic.ViewModels.Tag;
using Board.Models.Data;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.TagList
{
    public class TagListWindowViewModel
    {
        private readonly ITagListWindowAccess access;
        private readonly WallDocument document;
        private readonly int tableId;
        private readonly IDialogService dialogService;

        private void LoadTags()
        {
            this.Tags.Clear();

            var tagModels = document.Database.GetTags(tableId);
            foreach (var tag in tagModels)
            {
                var tagViewModel = new TagViewModel(tag);
                this.Tags.Add(tagViewModel);
            }
        }

        private void DoClose()
        {
            access.Close();
        }

        private void DoNewTag()
        {
            (bool result, TagModel newTag) = dialogService.ShowNewTagDialog();

            // TODO
        }


        public TagListWindowViewModel(ITagListWindowAccess access, WallDocument document, int tableId, IDialogService dialogService)
        {
            this.access = access;
            this.document = document;
            this.tableId = tableId;
            this.dialogService = dialogService;

            NewTagCommand = new AppCommand(obj => DoNewTag());
            CloseCommand = new AppCommand(obj => DoClose());

            Tags = new ObservableCollection<TagViewModel>();

            LoadTags();

            Result = 0;
        }

        public ICommand CloseCommand { get; }
        public ICommand NewTagCommand { get; }
        public ObservableCollection<TagViewModel> Tags { get; }
        public EditActions Result { get; private set; }
    }
}
