using Board.BusinessLogic.Models.Dialogs;
using Board.BusinessLogic.Models.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.Resources;
using Board.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Services.DialogService
{
    internal class DialogService : IDialogService
    {
        public (bool result, string filename) ShowOpenDialog(string filter = null, string title = null, string filename = null)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.DefaultDialogTitle;

            if (dialog.ShowDialog() == true)
                return new (true, dialog.FileName);
            else
                return new (false, null);
        }

        public (bool result, string filename) ShowSaveDialog(string filter = null, string title = null, string filename = null)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.DefaultDialogTitle;

            if (dialog.ShowDialog() == true)
                return (true, dialog.FileName);
            else
                return (false, null);
        }

        public (bool result, string path) ShowBrowseFolderDialog(string title = null, string path = null)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog
            {
                Title = title,
                EnsurePathExists = true,
                DefaultDirectory = path,
                IsFolderPicker = true
            };

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                return (true, fileDialog.FileName);
            else
                return (false, null);
        }

        public (bool result, SQLiteConfigResult data) ShowSQLiteDataDialog(SQLiteConfigResult data = null)
        {
            SQLiteConfigWindow dialog = new SQLiteConfigWindow(data);
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }

        public (bool result, DocumentInfo data) ShowNewWallDialog()
        {
            NewWallWindow dialog = new NewWallWindow();
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }
    }
}
