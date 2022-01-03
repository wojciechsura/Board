using Board.BusinessLogic.ViewModels.DeleteDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;
using Unity.Resolution;

namespace Board.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy DeleteDialogWindow.xaml
    /// </summary>
    public partial class DeleteDialogWindow : Window, IDeleteDialogWindowAccess
    {
        private readonly DeleteDialogWindowViewModel viewModel;

        public DeleteDialogWindow(string message)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<DeleteDialogWindowViewModel>(new ParameterOverride("access", this),
                new ParameterOverride("message", message));
            DataContext = viewModel;
        }

        public bool DeletePermanently => viewModel.DeletePermanently;

        public void Close(bool result)
        {
            DialogResult = result;
            Close();
        }
    }
}
