using Board.BusinessLogic.ViewModels.Document;
using Board.Common.Wpf.Controls;
using Board.Utils;
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
    /// Interaction logic for Entry.xaml
    /// </summary>
    public partial class Entry : Border
    {
        private bool inDragDrop;
        private bool lBtnDown;
        private EntryViewModel viewModel;
        private DragAdorner dragAdorner;

        public Entry()
        {
            InitializeComponent();
            inDragDrop = false;
            lBtnDown = false;
        }

        private void HandleHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            inDragDrop = false;

            if (e.ChangedButton == MouseButton.Left && sender is Label)
                lBtnDown = true;
        }

        private void HandleHeaderMouseMove(object sender, MouseEventArgs e)
        {
            if (lBtnDown &&
                e.LeftButton == MouseButtonState.Pressed &&
                sender is Label lSender &&
                viewModel != null &&
                viewModel.CanDragDrop)
            {
                FrameworkElement parent = lSender;
                
                while (parent is not null && parent is not Table)
                    parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;

                dragAdorner = new DragAdorner(this, e.GetPosition(this));

                var adornerLayer = AdornerLayer.GetAdornerLayer(parent);

                adornerLayer.Add(dragAdorner);
                DragDrop.DoDragDrop(lSender, viewModel, DragDropEffects.Move);
                adornerLayer.Remove(dragAdorner);
            }
        }

        private void HandleHeaderMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (inDragDrop)
                {
                    inDragDrop = false;
                }

                lBtnDown = false;
            }
        }

        private void HandleHeaderGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (dragAdorner != null)
            {
                Label label = sender as Label;
                var pos = label.PointFromScreen(Win32.AbsoluteMousePosition);

                dragAdorner.UpdatePosition(pos);
            }
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (EntryViewModel)e.NewValue;
        }
    }
}
