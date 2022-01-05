using Board.BusinessLogic.Infrastructure.Collections;
using Board.Models.Data;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class EntryViewModel : BaseEntryViewModel
    {
        private readonly EntryModel entry;

        public EntryViewModel(EntryModel entry, IDocumentHandler handler)
            : base(handler)
        {
            this.entry = entry;

            DeleteEntryCommand = new AppCommand(obj => handler.DeleteEntryRequest(this));
            EditEntryCommand = new AppCommand(obj => handler.EditEntryRequest(this));
        }

        public string Title => entry.Title;

        public ICommand DeleteEntryCommand { get; }
        public ICommand EditEntryCommand { get; }
        public EntryModel Entry => entry;
    }
}
