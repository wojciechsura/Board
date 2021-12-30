using Board.BusinessLogic.Models.Dialogs;
using Board.BusinessLogic.Models.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.NewWall
{
    public class NewWallWindowViewModel : BaseViewModel
    {
        private readonly INewWallWindowAccess access;
        private readonly IDialogService dialogService;

        private void DoSQLite()
        {
            (bool result, SQLiteConfigResult data) = dialogService.ShowSQLiteDataDialog();

            if (result)
            {
                var databaseDef = new SQLiteDatabaseDefinition();
                var filesystemDef = new LocalFilesystemDefinition();
                var path = System.IO.Path.Combine(data.Path, data.WallName);

                var definition = new DocumentDefinition
                {
                    Database = databaseDef,
                    Filesystem = filesystemDef
                };

                var info = new DocumentInfo(path, definition);
                Result = info;

                access.Close(true);
            }
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        public NewWallWindowViewModel(INewWallWindowAccess access, IDialogService dialogService)
        {
            this.access = access;
            this.dialogService = dialogService;

            SQLiteCommand = new AppCommand(obj => DoSQLite());
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        public DocumentInfo Result { get; private set; }

        public ICommand SQLiteCommand { get; }

        public ICommand CancelCommand { get; }
    }
}
