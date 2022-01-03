using Board.BusinessLogic.Infrastructure.Collections;
using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Document
{
    public class ColumnViewModel : BaseViewModel, IParentedItem<TableViewModel>
    {
        private readonly ColumnModel column;
        private readonly IDocumentHandler handler;
        private readonly ObservableParentedCollection<EntryViewModel, ColumnViewModel> entries;

        public ColumnViewModel(ColumnModel column, List<EntryViewModel> entries, IDocumentHandler handler)
        {
            this.entries = new(this);

            this.column = column;
            this.handler = handler;
            foreach (var entry in entries)
                this.entries.Add(entry);

            EditColumnCommand = new AppCommand(obj => handler.EditColumnRequest(this));
            DeleteColumnCommand = new AppCommand(obj => handler.DeleteColumnRequest(this));
        }

        public TableViewModel Parent { get; set; }

        public ObservableParentedCollection<EntryViewModel, ColumnViewModel> Entries => entries;

        public ICommand EditColumnCommand { get; }
        public ICommand DeleteColumnCommand { get; }

        public string Name => column.Name;

        public ColumnModel Column => column;
    }
}
