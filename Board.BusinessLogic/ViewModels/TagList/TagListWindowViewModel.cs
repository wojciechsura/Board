using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.Services.EventBus;
using Board.BusinessLogic.Types.Enums;
using Board.Models.Data;
using Board.Models.Events;
using Board.Models.Types;
using Board.Resources;
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
    public class TagListWindowViewModel : ITagHandler
    {
        // Private fields -----------------------------------------------------

        private readonly ITagListWindowAccess access;
        private readonly WallDocument document;
        private readonly int tableId;
        private readonly IDialogService dialogService;
        private readonly IEventBus eventBus;

        // Private methods ----------------------------------------------------

        private void LoadTags()
        {
            this.Tags.Clear();

            var tagModels = document.Database.GetTags(tableId, false);
            foreach (var tag in tagModels)
            {
                var tagViewModel = new TagViewModel(tag, this);
                this.Tags.Add(tagViewModel);
            }
        }

        private void DoClose()
        {
            access.Close();
        }

        private void InsertTag(TagViewModel tag)
        {
            int i = 0;

            while (i < Tags.Count && string.Compare(Tags[i].Name, tag.Name) < 0)
                i++;

            Tags.Insert(i, tag);
        }

        private void RemoveTag(TagViewModel tag)
        {
            Tags.Remove(tag);
        }

        private void DoNewTag()
        {
            (bool result, TagModel newTag) = dialogService.ShowNewTagDialog();

            newTag.TableId = tableId;
            document.Database.AddTag(newTag);

            var tagViewModel = new TagViewModel(newTag, this);
            InsertTag(tagViewModel);

            eventBus.Send(new TagChangedEvent(ChangeKind.Add, tableId, newTag.Id));
        }

        // ITagHandler implementation -----------------------------------------

        void ITagHandler.RequestEdit(TagViewModel tagViewModel)
        {
            TagModel tagModel = tagViewModel.Tag;

            bool result = dialogService.ShowEditTagDialog(tagModel);
            if (result)
            {
                document.Database.UpdateTag(tagModel);

                var newTagViewModel = new TagViewModel(tagModel, this);
                RemoveTag(tagViewModel);
                InsertTag(newTagViewModel);

                eventBus.Send(new TagChangedEvent(ChangeKind.Edit, tableId, tagModel.Id));
            }
        }

        void ITagHandler.RequestDelete(TagViewModel tagViewModel)
        {
            var message = string.Format(Strings.Message_TagDeletion, tagViewModel.Name);
            (bool result, bool? permanently) = dialogService.ShowDeleteDialog(message);
            if (result)
            {
                document.Database.DeleteTag(tagViewModel.Tag.Id, permanently.Value);
                RemoveTag(tagViewModel);

                eventBus.Send(new TagChangedEvent(ChangeKind.Delete, tableId, tagViewModel.Tag.Id));
            }
        }

        // Public methods -----------------------------------------------------

        public TagListWindowViewModel(ITagListWindowAccess access, WallDocument document, int tableId, IDialogService dialogService, IEventBus eventBus)
        {
            this.access = access;
            this.document = document;
            this.tableId = tableId;
            this.dialogService = dialogService;
            this.eventBus = eventBus;

            NewTagCommand = new AppCommand(obj => DoNewTag());
            CloseCommand = new AppCommand(obj => DoClose());

            Tags = new ObservableCollection<TagViewModel>();

            LoadTags();
        }

        // Public properties --------------------------------------------------

        public ICommand CloseCommand { get; }
        public ICommand NewTagCommand { get; }
        public ObservableCollection<TagViewModel> Tags { get; }
    }
}
