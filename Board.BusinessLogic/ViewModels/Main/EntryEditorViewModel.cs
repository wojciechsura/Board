using Board.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Board.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Board.BusinessLogic.Infrastructure.Document;
using System.Windows.Input;
using Spooksoft.VisualStateManager.Commands;
using System.Collections.ObjectModel;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class EntryEditorViewModel : BaseViewModel, IEntryEditorHandler
    {
        // Private fields -----------------------------------------------------

        private readonly int entryId;
        private readonly int tableId;
        private readonly WallDocument document;
        private readonly IDocumentHandler handler;

        private bool isEditingTitle = false;
        private bool isEditingDescription = false;

        [SyncWithModel(nameof(EntryModel.Title))]
        private string title;
        [SyncWithModel(nameof(EntryModel.Description))]
        private string description;

        // Private methods ----------------------------------------------------

        private void InsertTag<TTag>(ObservableCollection<TTag> tags, TTag tag)
            where TTag : BaseTagViewModel
        {
            int i = 0;
            while (i < tags.Count && string.Compare(tags[i].Name, tag.Name) < 0)
                i++;

            tags.Insert(i, tag);
        }

        private void UpdateFromModel(EntryEditModel model)
        {
            base.UpdateFromModel(model);

            var tags = document.Database.GetTags(tableId, false)
                .OrderBy(t => t.Name);

            foreach (var tag in tags)
            {
                AvailableTagViewModel availableTag = new AvailableTagViewModel(tag, this);

                if (model.Tags.Any(t => t.Id == tag.Id))
                {
                    AddedTags.Add(new AddedTagViewModel(tag, this));
                    availableTag.IsSelected = true;
                }

                AvailableTags.Add(availableTag);
            }

            Comments.Add(new NewInplaceCommentViewModel(handler, new CommentModel(), true));

            foreach (var comment in model.Comments.OrderByDescending(c => c.Added))
            {
                CommentViewModel commentViewModel = new(comment, handler);
                Comments.Add(commentViewModel);
            }
        }

        // IEntryEditorHandler ------------------------------------------------

        void IEntryEditorHandler.ToggleTag(AvailableTagViewModel tag)
        {
            if (tag.IsSelected)
            {
                document.Database.RemoveTagFromEntry(entryId, tag.Tag.Id);
                
                tag.IsSelected = false;

                var addedTag = AddedTags.FirstOrDefault(t => t.Tag.Id == tag.Tag.Id);
                AddedTags.Remove(addedTag);
            }
            else
            {
                document.Database.AddTagToEntry(entryId, tag.Tag.Id);

                var addedTag = new AddedTagViewModel(tag.Tag, this);
                InsertTag(AddedTags, addedTag);
                tag.IsSelected = true;
            }
        }

        // Public methods -----------------------------------------------------

        public EntryEditorViewModel(int entryId, 
            int tableId,
            EntryViewModel editedEntryViewModel,
            WallDocument document,
            IDocumentHandler handler)
        {
            var editEntryModel = document.Database.GetEntryEdit(entryId);

            this.entryId = entryId;
            this.tableId = tableId;
            this.document = document;
            this.handler = handler;

            CloseCommand = new AppCommand(obj => handler.RequestEditorClose(editedEntryViewModel));

            AddedTags = new ();
            AvailableTags = new ();
            Comments = new();

            UpdateFromModel(editEntryModel);
        }

        public void SetTitle(string title)
        {
            var model = document.Database.GetEntryById(entryId);
            model.Title = title;
            document.Database.UpdateEntry(model);

            Title = title;
        }

        public void SetDescription(string description)
        {
            var model = document.Database.GetEntryById(entryId);
            model.Description = description;
            document.Database.UpdateEntry(model);

            Description = description;
        }

        // Public properties --------------------------------------------------

        public string Title
        {
            get => title;
            private set => Set(ref title, value);
        }

        public string Description
        {
            get => description;
            set => Set(ref description, value);
        } 

        public bool IsEditingTitle
        {
            get => isEditingTitle;
            set => Set(ref isEditingTitle, value);
        }

        public bool IsEditingDescription
        {
            get => isEditingDescription;
            set => Set(ref isEditingDescription, value);
        }

        public ObservableCollection<AddedTagViewModel> AddedTags { get; }

        public ObservableCollection<AvailableTagViewModel> AvailableTags { get; }

        public ObservableCollection<BaseCommentViewModel> Comments { get; }

        public ICommand CloseCommand { get; }
    }
}
