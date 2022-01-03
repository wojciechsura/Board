using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.ViewModels.ColumnEditor;
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
    /// Interaction logic for ColumnEditorWindow.xaml
    /// </summary>
    public partial class ColumnEditorWindow : Window, IColumnEditorWindowAccess
    {
        // Private fields -----------------------------------------------------

        private readonly ColumnEditorWindowViewModel viewModel;

        // IColumnEditorWindowAccess implementation ---------------------------

        void IColumnEditorWindowAccess.Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        // Public methods -----------------------------------------------------

        public ColumnEditorWindow(ColumnModel column, bool isNew)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ColumnEditorWindowViewModel>(new ParameterOverride("access", this),
                new ParameterOverride("column", column),
                new ParameterOverride("isNew", isNew));
            DataContext = viewModel;
        }

        // Public properties --------------------------------------------------

        public ColumnModel Result => viewModel.Result;
    }
}
