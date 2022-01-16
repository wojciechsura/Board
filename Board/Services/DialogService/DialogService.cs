using Board.Models.Data;
using Board.Models.Dialogs;
using Board.Models.Document;
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
using System.Windows;
using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Types.Enums;

namespace Board.Services.DialogService
{
    internal class DialogService : IDialogService
    {
        private readonly Stack<Window> dialogWindows = new();

        private void ActivateLastDialog()
        {
            if (dialogWindows.Any())
                dialogWindows.Peek().Activate();
        }

        private void PopDialog(Window dialog)
        {
            if (dialogWindows.Peek() != dialog)
                throw new InvalidOperationException("Broken dialog window stack mechanism!");

            dialogWindows.Pop();
        }

        private Window GetOwnerWindow()
        {
            return dialogWindows.Any() ? dialogWindows.Peek() : Application.Current.MainWindow;
        }

        public (bool result, string filename) ShowOpenDialog(string filter = null, string title = null, string filename = null)
        {
            try
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
                    return (true, dialog.FileName);
                else
                    return (false, null);
            }
            finally
            {
                ActivateLastDialog();
            }
        }

        public (bool result, string filename) ShowSaveDialog(string filter = null, string title = null, string filename = null)
        {
            try
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
            finally
            {
                ActivateLastDialog();
            }
        }

        public (bool result, string path) ShowBrowseFolderDialog(string title = null, string path = null)
        {
            try
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
            finally
            {
                ActivateLastDialog();
            }
        }

        public (bool result, SQLiteConfigResult data) ShowSQLiteDataDialog(SQLiteConfigResult data = null)
        {
            SQLiteConfigWindow dialog = new SQLiteConfigWindow(data);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return (true, dialog.Result);
                else
                    return (false, null);
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public (bool result, DocumentInfo data) ShowNewWallDialog()
        {
            NewWallWindow dialog = new NewWallWindow();
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return (true, dialog.Result);
                else
                    return (false, null);
            }
            finally
            {
                PopDialog(dialog);                
                ActivateLastDialog();
            }
        }

        public (bool result, TableEditModel model) ShowNewTableDialog()
        {
            TableEditorWindow dialog = new TableEditorWindow(new TableEditModel(), true);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return (true, dialog.Result);
                else
                    return (false, null);
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public bool ShowEditTableDialog(TableEditModel tableModel)
        {
            TableEditorWindow dialog = new TableEditorWindow(tableModel, false);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return true;
                else
                    return false;
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public (bool result, bool? permanently) ShowDeleteDialog(string message)
        {
            DeleteDialogWindow dialog = new DeleteDialogWindow(message);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return (true, dialog.DeletePermanently);
                else
                    return (false, null);
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public (bool result, ColumnModel model) ShowNewColumnDialog()
        {
            ColumnEditorWindow dialog = new ColumnEditorWindow(new ColumnModel(), true);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return (true, dialog.Result);
                else
                    return (false, null);
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public bool ShowEditColumnDialog(ColumnModel columnModel)
        {
            ColumnEditorWindow dialog = new ColumnEditorWindow(columnModel, false);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return true;
                else
                    return false;
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public void ShowEditTagsDialog(WallDocument document, int tableId)
        {
            TagListWindow dialog = new TagListWindow(document, tableId);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                dialog.ShowDialog();
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public (bool result, TagModel model) ShowNewTagDialog()
        {
            TagEditorWindow dialog = new TagEditorWindow(new TagModel(), true);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return (true, dialog.Result);
                else
                    return (false, null);
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public bool ShowEditTagDialog(TagModel tagModel)
        {
            TagEditorWindow dialog = new TagEditorWindow(tagModel, false);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                    return true;
                else
                    return false;
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }
    }
}
