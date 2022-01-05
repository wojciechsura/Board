using Board.Models.Data;
using Board.Models.Dialogs;
using Board.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Services.Dialogs
{
    public interface IDialogService
    {
        (bool result, string filename) ShowOpenDialog(string filter = null, string title = null, string filename = null);
        (bool result, string filename) ShowSaveDialog(string filter = null, string title = null, string filename = null);
        (bool result, string path) ShowBrowseFolderDialog(string title = null, string path = null);
        (bool result, SQLiteConfigResult data) ShowSQLiteDataDialog(SQLiteConfigResult data = null);
        (bool result, DocumentInfo data) ShowNewWallDialog();
        (bool result, TableModel model) ShowNewTableDialog();
        bool ShowEditTableDialog(TableModel tableModel);
        (bool result, bool? permanently) ShowDeleteDialog(string message);
        (bool result, ColumnModel model) ShowNewColumnDialog();
        bool ShowEditColumnDialog(ColumnModel columnModel);
    }
}
