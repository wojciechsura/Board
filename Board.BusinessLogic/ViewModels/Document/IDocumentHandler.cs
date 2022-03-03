using Board.BusinessLogic.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public interface IDocumentHandler : INotifyPropertyChanged
    {
        void NewColumnRequest(TableViewModel tableViewModel);
        void EditColumnRequest(ColumnViewModel columnViewModel);
        void DeleteColumnRequest(ColumnViewModel columnViewModel);
        void SaveNewInplaceEntryRequest(NewInplaceEntryViewModel newInplaceEntryViewModel);
        void CancelNewInplaceEntryRequest(NewInplaceEntryViewModel newInplaceEntryViewModel);
        void NewInplaceEntryRequest(ColumnViewModel columnViewModel);
        void DeleteEntryRequest(EntryViewModel entryViewModel);
        void EditEntryRequest(EntryViewModel entryViewModel);
        void RequestApplyFilter(TableViewModel tableViewModel);
        void RequestEditorClose(EntryViewModel entryToUpdate);
        void RequestMoveEntry(EntryViewModel entryViewModel, ColumnViewModel targetColumnViewModel, int newIndex);
        void RequestMoveColumn(ColumnViewModel columnViewModel, TableViewModel targetTableViewModel, int newIndex);
        void LoadMoreEntries(ColumnViewModel columnViewModel);

        bool ShowEntryDetails { get; }
    }
}
