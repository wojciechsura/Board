using Board.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class MainWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;

        private string data;

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access)
        {
            this.access = access;

            data = "Hello, world!";
        }

        // Public properties --------------------------------------------------

        public string Data
        {
            get => data;
            set => Set(ref data, value);
        }
    }
}
