using Board.Models.Data;
using Board.BusinessLogic.Types.Attributes;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class NewInplaceEntryViewModel : BaseEntryViewModel
    {
        private readonly EntryModel entry;

        public NewInplaceEntryViewModel(IDocumentHandler handler)
            : base(handler)
        {
            entry = new EntryModel();

            SaveCommand = new AppCommand(obj => handler.SaveNewInplaceEntryRequest(this));
            CancelCommand = new AppCommand(obj => handler.CancelNewInplaceEntryRequest(this));
        }
        
        public string Title
        {
            get => entry.Title;
            set => entry.Title = value;
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EntryModel Entry => entry;
    }
}
