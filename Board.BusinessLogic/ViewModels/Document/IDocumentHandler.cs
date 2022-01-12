using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Document
{
    public interface IDocumentHandler
    {
        void NewColumnRequest(TableViewModel tableViewModel);
        void EditColumnRequest(ColumnViewModel columnViewModel);
        void DeleteColumnRequest(ColumnViewModel columnViewModel);
        void SaveNewInplaceEntryRequest(NewInplaceCommentViewModel newInplaceEntryViewModel);
        void CancelNewInplaceEntryRequest(NewInplaceCommentViewModel newInplaceEntryViewModel);
        void NewInplaceEntryRequest(ColumnViewModel columnViewModel);
        void DeleteEntryRequest(EntryViewModel entryViewModel);
        void EditEntryRequest(EntryViewModel entryViewModel);
        void RequestEditorClose(EntryViewModel entryToUpdate);
        void RequestMoveEntry(EntryViewModel entryViewModel, ColumnViewModel targetColumnViewModel, int newIndex);
        void RequestMoveColumn(ColumnViewModel columnViewModel, TableViewModel targetTableViewModel, int newIndex);
    }
}
