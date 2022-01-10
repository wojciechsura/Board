using Board.BusinessLogic.Infrastructure.Document;
using Board.BusinessLogic.Types.Enums;
using Board.BusinessLogic.ViewModels.TagList;
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
    /// Logika interakcji dla klasy TagListWindow.xaml
    /// </summary>
    public partial class TagListWindow : Window, ITagListWindowAccess
    {
        private readonly TagListWindowViewModel viewModel;

        void ITagListWindowAccess.Close()
        {
            Close();
        }

        public TagListWindow(WallDocument document, int tableId)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<TagListWindowViewModel>(new ParameterOverride("access", this),
                new ParameterOverride("tableId", tableId),
                new ParameterOverride("document", document));
            DataContext = viewModel;            
        }        
    }
}
