using Board.BusinessLogic.Models.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class MainWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;

        // Private methods ----------------------------------------------------

        private void DoNew()
        {
            (bool result, DocumentDefinition definition) = dialogService.ShowNewWallDialog();
            
            if (result)
            {
                // TODO
            }            
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, IDialogService dialogService)
        {
            this.access = access;
            this.dialogService = dialogService;

            NewCommand = new AppCommand(obj => DoNew());
        }

        // Public properties --------------------------------------------------

        public ICommand NewCommand { get; }
    }
}
