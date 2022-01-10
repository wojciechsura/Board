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
    public class EntryEditorViewModel : ModelEditorViewModel<EntryEditModel>, IEntryEditorHandler
    {
        // Private fields -----------------------------------------------------

        private readonly int entryId;
        private readonly int tableId;
        private readonly WallDocument document;
        private readonly IDocumentHandler handler;

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

        // Protected methods --------------------------------------------------

        protected override void UpdateFromModel(EntryEditModel model)
        {
            base.UpdateFromModel(model);

            var tags = document.Database.GetTags(tableId, false)
                .OrderBy(t => t.Name);

            foreach (var tag in tags)
            {
                if (model.Tags.Any(t => t.Id == tag.Id))
                    AddedTags.Add(new AddedTagViewModel(tag, this));
                else
                    AvailableTags.Add(new AvailableTagViewModel(tag, this));
            }
        }

        // IEntryEditorHandler ------------------------------------------------

        void IEntryEditorHandler.AddTag(AvailableTagViewModel tag)
        {
            document.Database.AddTagToEntry(entryId, tag.Tag.Id);

            var addedTag = new AddedTagViewModel(tag.Tag, this);
            AvailableTags.Remove(tag);
            InsertTag(AddedTags, addedTag);
        }

        void IEntryEditorHandler.RemoveTag(AddedTagViewModel tag)
        {
            document.Database.RemoveTagFromEntry(entryId, tag.Tag.Id);

            var availableTag = new AvailableTagViewModel(tag.Tag, this);
            AddedTags.Remove(tag);
            InsertTag(AvailableTags, availableTag);
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

            AddedTags = new ObservableCollection<AddedTagViewModel>();
            AvailableTags = new ObservableCollection<AvailableTagViewModel>();

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

        public ObservableCollection<AddedTagViewModel> AddedTags { get; }
        public ObservableCollection<AvailableTagViewModel> AvailableTags { get; }

        public ICommand CloseCommand { get; }
    }
}
