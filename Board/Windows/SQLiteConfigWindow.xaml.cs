using Board.BusinessLogic.Models.Dialogs;
using Board.BusinessLogic.ViewModels.SQLiteConfig;
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
    /// Interaction logic for SQLiteConfigWindow.xaml
    /// </summary>
    public partial class SQLiteConfigWindow : Window, ISQLiteConfigWindowAccess
    {
        private readonly SQLiteConfigWindowViewModel viewModel;

        public SQLiteConfigWindow(SQLiteConfigResult data)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<SQLiteConfigWindowViewModel>(new ParameterOverride("access", this),
                new ParameterOverride("data", data));
            DataContext = viewModel;
        }

        void ISQLiteConfigWindowAccess.Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public SQLiteConfigResult Result => viewModel.Result;
    }
}
