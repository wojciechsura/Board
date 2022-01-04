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
        private readonly ObservableParentedCollection<BaseEntryViewModel, ColumnViewModel> entries;

        public ColumnViewModel(ColumnModel column, List<BaseEntryViewModel> entries, IDocumentHandler handler)
        {
            this.entries = new(this);

            this.column = column;
            this.handler = handler;
            foreach (var entry in entries)
                this.entries.Add(entry);

            EditColumnCommand = new AppCommand(obj => handler.EditColumnRequest(this));
            DeleteColumnCommand = new AppCommand(obj => handler.DeleteColumnRequest(this));
            NewInplaceEntryCommand = new AppCommand(obj => handler.NewInplaceEntryRequest(this));
        }

        public TableViewModel Parent { get; set; }

        public ObservableParentedCollection<BaseEntryViewModel, ColumnViewModel> Entries => entries;

        public ICommand EditColumnCommand { get; }
        public ICommand DeleteColumnCommand { get; }
        public ICommand NewInplaceEntryCommand { get; }

        public string Name => column.Name;

        public ColumnModel Column => column;
    }
}
