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
using System.Collections.ObjectModel;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class EntryViewModel : BaseEntryViewModel
    {
        private readonly EntryDisplayModel entry;

        public EntryViewModel(EntryDisplayModel entry, IDocumentHandler handler)
            : base(handler)
        {
            this.entry = entry;

            Tags = new ObservableCollection<TagViewModel>();
            foreach (var tag in entry.Tags)
            {
                var tagViewModel = new TagViewModel(tag);
                Tags.Add(tagViewModel);
            }

            DeleteEntryCommand = new AppCommand(obj => handler.DeleteEntryRequest(this));
            EditEntryCommand = new AppCommand(obj => handler.EditEntryRequest(this));
        }

        public string Title => entry.Title;

        public ICommand DeleteEntryCommand { get; }
        public ICommand EditEntryCommand { get; }
        public EntryDisplayModel Entry => entry;

        public ObservableCollection<TagViewModel> Tags { get; }
    }
}
