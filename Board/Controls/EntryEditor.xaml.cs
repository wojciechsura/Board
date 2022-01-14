using Board.BusinessLogic.ViewModels.Main;
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
    /// Logika interakcji dla klasy EntryEditor.xaml
    /// </summary>
    public partial class EntryEditor : Border
    {
        // Private fields -----------------------------------------------------

        private EntryEditorViewModel viewModel;

        // Private methods ----------------------------------------------------

        private void BeginTitleEdit()
        {
            tbTitleEditor.Text = viewModel.Title;
            viewModel.IsEditingTitle = true;
        }

        private void CancelTitleEdit()
        {
            viewModel.IsEditingTitle = false;
        }

        private void CommitTitleEdit()
        {
            viewModel.SetTitle(tbTitleEditor.Text);
            viewModel.IsEditingTitle = false;
        }

        private void BeginDescriptionEdit()
        {
            tbDescription.Text = viewModel.Description;
            viewModel.IsEditingDescription = true;
        }

        private void CancelDescriptionEdit()
        {
            viewModel.IsEditingDescription = false;
        }

        private void CommitDescriptionEdit()
        {
            viewModel.SetDescription(tbDescription.Text);
            viewModel.IsEditingDescription = false;
        }

        private void CancelTitleClick(object sender, RoutedEventArgs e)
        {
            CancelTitleEdit();
        }

        private void EditTitleClick(object sender, RoutedEventArgs e)
        {
            BeginTitleEdit();
        }

        private void SaveTitleClick(object sender, RoutedEventArgs e)
        {
            CommitTitleEdit();
        }

        private void TitleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CancelTitleEdit();
            }
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;
                CommitTitleEdit();
            }
        }

        private void TitleLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            BeginTitleEdit();
        }

        private void TitleLostFocus(object sender, RoutedEventArgs e)
        {
            CommitTitleEdit();
        }

        private void EditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (EntryEditorViewModel)e.NewValue;
        }

        private void EditDescriptionClick(object sender, RoutedEventArgs e)
        {
            BeginDescriptionEdit();
        }

        private void CancelDescriptionClick(object sender, RoutedEventArgs e)
        {
            CancelDescriptionEdit();
        }

        private void SaveDescriptionClick(object sender, RoutedEventArgs e)
        {
            CommitDescriptionEdit();
        }

        private void DescriptionLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            BeginDescriptionEdit();
        }

        private void HandleAddTagsButtonClick(object sender, RoutedEventArgs e)
        {
            tpAddTags.IsOpen = true;
        }

        private void HandleTagsButtonClick(object sender, RoutedEventArgs e)
        {
            tpTags.IsOpen = true;
        }

        private void HandleDatesButtonClick(object sender, RoutedEventArgs e)
        {
            dpDates.IsOpen = true;
        }

        // Public methods -----------------------------------------------------

        public EntryEditor()
        {
            InitializeComponent();
        }
    }
}
