using Board.Models.Document;
using Board.BusinessLogic.Services.Dialogs;
using Board.BusinessLogic.ViewModels.NewWall;
using Spooksoft.VisualStateManager.Commands;
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
    /// Interaction logic for NewWallWindow.xaml
    /// </summary>
    public partial class NewWallWindow : Window, INewWallWindowAccess
    {
        private readonly NewWallWindowViewModel viewModel;

        public NewWallWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<NewWallWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;
        }

        public void Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public DocumentInfo Result => viewModel.Result;
    }
}
