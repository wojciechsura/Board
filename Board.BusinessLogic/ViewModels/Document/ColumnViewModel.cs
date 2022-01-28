using Board.BusinessLogic.Infrastructure.Collections;
using Board.Models.Data;
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

        private bool canLoadMore;

        public ColumnViewModel(ColumnModel column, List<BaseEntryViewModel> entries, IDocumentHandler handler, bool canLoadMore)
        {
            this.entries = new(this);

            this.column = column;
            this.handler = handler;
            this.canLoadMore = canLoadMore;

            foreach (var entry in entries)
                this.entries.Add(entry);

            EditColumnCommand = new AppCommand(obj => handler.EditColumnRequest(this));
            DeleteColumnCommand = new AppCommand(obj => handler.DeleteColumnRequest(this));
            NewInplaceEntryCommand = new AppCommand(obj => handler.NewInplaceEntryRequest(this));
            LoadMoreEntriesCommand = new AppCommand(obj => handler.LoadMoreEntries(this));
        }

        public void RequestMoveEntry(EntryViewModel entryViewModel, int newIndex)
        {
            handler.RequestMoveEntry(entryViewModel, this, newIndex);
        }

        public TableViewModel Parent { get; set; }

        public ObservableParentedCollection<BaseEntryViewModel, ColumnViewModel> Entries => entries;

        public ICommand EditColumnCommand { get; }
        public ICommand DeleteColumnCommand { get; }
        public ICommand NewInplaceEntryCommand { get; }
        public ICommand LoadMoreEntriesCommand { get; }

        public string Name => column.Name;

        public ColumnModel Column => column;

        public bool DimItems => column.DimItems;

        public bool CanLoadMore
        {
            get => canLoadMore;
            set => Set(ref canLoadMore, value);
        }
    }
}
