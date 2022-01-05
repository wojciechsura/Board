using Board.Models.Dialogs;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.SQLiteConfig
{
    public class SQLiteConfigWindowViewModel : BaseViewModel
    {
        private readonly ISQLiteConfigWindowAccess access;
        private readonly IDialogService dialogService;

        private string wallName;
        private string path;

        private readonly BaseCondition pathExistsCondition;
        private readonly BaseCondition nameValidCondition;
        private readonly BaseCondition fileAlreadyExistsCondition;

        private void DoOpenFolder()
        {
            (bool result, string path) = dialogService.ShowBrowseFolderDialog();
            if (result)
            {
                if (!path.EndsWith("\\"))
                    path += "\\";

                this.Path = path;
            }
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        private void DoOk()
        {
            access.Close(true);
        }


        public SQLiteConfigWindowViewModel(ISQLiteConfigWindowAccess access, IDialogService dialogService, SQLiteConfigResult data)
        {
            this.access = access;
            this.dialogService = dialogService;

            if (data != null)
            {
                Path = data.Path;
                WallName = data.WallName;
            }

            pathExistsCondition = new ChainedLambdaCondition<SQLiteConfigWindowViewModel>(this, vm => System.IO.Directory.Exists(vm.Path), false);
            nameValidCondition = new ChainedLambdaCondition<SQLiteConfigWindowViewModel>(this, vm => !string.IsNullOrEmpty(vm.WallName) && vm.WallName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0, false);
            fileAlreadyExistsCondition = new ChainedLambdaCondition<SQLiteConfigWindowViewModel>(this, vm => System.IO.Directory.Exists(vm.Path + vm.WallName), false);

            OpenFolderCommand = new AppCommand(obj => DoOpenFolder());
            OkCommand = new AppCommand(obj => DoOk(), pathExistsCondition & nameValidCondition & !fileAlreadyExistsCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        public string WallName 
        {
            get => wallName;
            set => Set(ref wallName, value);
        }

        public string Path
        {
            get => path;
            set => Set(ref path, value);
        }

        public ICommand OpenFolderCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public SQLiteConfigResult Result => new SQLiteConfigResult(WallName, Path);
    }
}
