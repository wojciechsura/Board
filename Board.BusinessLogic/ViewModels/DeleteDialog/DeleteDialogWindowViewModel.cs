using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.DeleteDialog
{
    public class DeleteDialogWindowViewModel : BaseViewModel
    {
        private readonly IDeleteDialogWindowAccess access;
        private bool deletePermanently;

        private void DoNo()
        {
            access.Close(false);
        }

        private void DoYes()
        {
            access.Close(true);
        }

        public DeleteDialogWindowViewModel(IDeleteDialogWindowAccess access, string message)
        {
            this.access = access;
            Message = message;

            YesCommand = new AppCommand(obj => DoYes());
            NoCommand = new AppCommand(obj => DoNo());
        }

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }

        public string Message { get; }

        public bool DeletePermanently
        {
            get => deletePermanently;
            set => Set(ref deletePermanently, value);
        }
    }
}
