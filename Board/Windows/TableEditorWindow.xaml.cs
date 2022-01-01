using Board.BusinessLogic.Models.Data;
using Board.BusinessLogic.ViewModels.TableEditor;
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
    /// Logika interakcji dla klasy BoardEditorWindow.xaml
    /// </summary>
    public partial class TableEditorWindow : Window, ITableEditorWindowAccess
    {
        // Private fields -----------------------------------------------------

        private readonly TableEditorWindowViewModel viewModel;

        // ITableEditorWindowAccess implementation ----------------------------

        void ITableEditorWindowAccess.Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        // Public methods -----------------------------------------------------

        public TableEditorWindow(TableModel table, bool isNew)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<TableEditorWindowViewModel>(new ParameterOverride("access", this),
                new ParameterOverride("table", table),
                new ParameterOverride("isNew", isNew));
            DataContext = viewModel;
        }

        // Public properties --------------------------------------------------

        public TableModel Result => viewModel.Result;
    }
}
