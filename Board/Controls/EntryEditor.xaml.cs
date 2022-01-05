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
        private EntryEditorViewModel viewModel;

        private void BeginTitleEdit()
        {
            tbTitleEditor.Text = viewModel.Title;
            IsEditingTitle = true;
        }

        private void CancelTitleEdit()
        {
            IsEditingTitle = false;
        }

        private void CommitTitleEdit()
        {
            viewModel.SetTitle(tbTitleEditor.Text);
            IsEditingTitle = false;
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

        private void EditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (EntryEditorViewModel)e.NewValue;
        }

        public EntryEditor()
        {
            InitializeComponent();
        }

        // Dependency properties ----------------------------------------------

        #region IsEditingTitle dependency property

        // Using a DependencyProperty as the backing store for IsEditingTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditingTitleProperty =
            DependencyProperty.Register("IsEditingTitle", typeof(bool), typeof(EntryEditor), new PropertyMetadata(false));

        public bool IsEditingTitle
        {
            get { return (bool)GetValue(IsEditingTitleProperty); }
            set { SetValue(IsEditingTitleProperty, value); }
        }

        #endregion
    }
}
