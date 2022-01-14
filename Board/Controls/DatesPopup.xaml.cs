using Board.BusinessLogic.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Board.Controls
{
    /// <summary>
    /// Interaction logic for DatesPopup.xaml
    /// </summary>
    public partial class DatesPopup : Popup
    {
        private EntryEditorViewModel viewModel;

        public DatesPopup()
        {
            InitializeComponent();
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (EntryEditorViewModel)e.NewValue;
        }

        private void HandleSaveDatesClick(object sender, RoutedEventArgs e)
        {
            viewModel.CommitDateChanges();
        }

        private void HandleCancelDatesClick(object sender, RoutedEventArgs e)
        {
            viewModel.CancelDateChanges();
        }
    }
}
