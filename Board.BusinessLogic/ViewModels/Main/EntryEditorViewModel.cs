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

namespace Board.BusinessLogic.ViewModels.Main
{
    public class EntryEditorViewModel : ModelEditorViewModel<EntryModel>
    {
        private readonly int entryId;
        private readonly WallDocument document;
        private readonly IDocumentHandler handler;

        [SyncWithModel(nameof(EntryModel.Title))]
        private string title;
        [SyncWithModel(nameof(EntryModel.Description))]
        private string description;

        public EntryEditorViewModel(int entryId, 
            EntryViewModel editedEntryViewModel,
            WallDocument document,
            IDocumentHandler handler)
        {
            var fullEntryModel = document.Database.GetFullEntryById(entryId);
            this.entryId = entryId;
            this.document = document;
            this.handler = handler;

            CloseCommand = new AppCommand(obj => handler.RequestEditorClose(editedEntryViewModel));

            UpdateFromModel(fullEntryModel);
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

        public ICommand CloseCommand { get; }
    }
}
