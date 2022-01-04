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
        void SaveNewInplaceEntryRequest(NewInplaceEntryViewModel newInplaceEntryViewModel);
        void CancelNewInplaceEntryRequest(NewInplaceEntryViewModel newInplaceEntryViewModel);
        void NewInplaceEntryRequest(ColumnViewModel columnViewModel);
        void DeleteEntryRequest(EntryViewModel entryViewModel);
    }
}
