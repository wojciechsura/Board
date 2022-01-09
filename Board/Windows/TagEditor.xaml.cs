using Board.BusinessLogic.ViewModels.TagEditor;
using Board.Models.Data;
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
    /// Logika interakcji dla klasy TagEditor.xaml
    /// </summary>
    public partial class TagEditorWindow : Window, ITagEditorWindowAccess
    {
        private readonly TagEditorWindowViewModel viewModel;

        void ITagEditorWindowAccess.Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public TagEditorWindow(TagModel tagModel, bool isNew)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<TagEditorWindowViewModel>(new ParameterOverride("access", this),
                new ParameterOverride("tag", tagModel),
                new ParameterOverride("isNew", isNew));
            DataContext = viewModel;
        }

        private void HandleColorRectangleMouseDown(object sender, MouseButtonEventArgs e)
        {
            pColor.IsOpen = true;
        }

        public TagModel Result => viewModel.Result;
    }
}
