using Board.BusinessLogic.ViewModels.Document;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Board.Controls
{
    /// <summary>
    /// Interaction logic for NewInplaceEntry.xaml
    /// </summary>
    public partial class NewInplaceEntry : Border
    {
        private NewInplaceEntryViewModel viewModel;

        public NewInplaceEntry()
        {
            InitializeComponent();
        }

        private void HandleTitleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewModel.SaveCommand.CanExecute(null))
                    viewModel.SaveCommand.Execute(null);

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (viewModel.CancelCommand.CanExecute(null))
                    viewModel.CancelCommand.Execute(null);

                e.Handled = true;
            }
        }

        private void HandleRootDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = e.NewValue as NewInplaceEntryViewModel;
        }
    }
}
