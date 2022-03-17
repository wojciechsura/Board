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
    /// Interaction logic for Column.xaml
    /// </summary>
    public partial class Column : Border
    {
        private const double ADORNER_HEIGHT = 5;
        private readonly Point POINT_ZERO = new Point(0, 0);

        private class EntryDropAdorner : Adorner
        {
            private Brush brush;
            private double locationY;

            public EntryDropAdorner(UIElement adornedElement, double locationY) 
                : base(adornedElement)
            {
                brush = new SolidColorBrush(Color.FromArgb(255, 30, 60, 90));
                this.locationY = locationY;
                IsHitTestVisible = false;
            }

            public void UpdatePosition(double locationY)
            {
                this.locationY = locationY;
                InvalidateVisual();
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRectangle(brush, null, new Rect(0, locationY - ADORNER_HEIGHT / 2, this.RenderSize.Width, ADORNER_HEIGHT));
            }
        }

        private ColumnViewModel viewModel;
        
        // Drag
        private DragAdorner dragAdorner;
        private bool inDragDrop;
        private bool lBtnDown;

        // Drop
        private EntryDropAdorner dropAdorner;
        private List<(double yStart, double height)> dropEntryBorders;

        public Column()
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
            viewModel = (ColumnViewModel)e.NewValue;
        }

        private void AddDropAdorner(ItemsControl items, double locationY)
        {
            dropAdorner = new EntryDropAdorner(items, locationY);
            var layer = AdornerLayer.GetAdornerLayer(items);
            layer.Add(dropAdorner);
        }

        private void RemoveDropAdorner(ItemsControl items)
        {
            var layer = AdornerLayer.GetAdornerLayer(items);
            layer.Remove(dropAdorner);
            dropAdorner = null;
        }

        private void EvalEntryBorders(ItemsControl items)
        {
            dropEntryBorders = new List<(double yStart, double height)>();

            for (int i = 0; i < items.Items.Count; i++)
            {
                FrameworkElement item = (FrameworkElement)items.ItemContainerGenerator.ContainerFromIndex(i);
                var topLeft = item.TranslatePoint(POINT_ZERO, items);

                dropEntryBorders.Add((topLeft.Y, item.ActualHeight));
            }
        }

        private (double adornerY, int newIndex) EvalDropData(ItemsControl items, Point localPosition)
        {
            if (dropEntryBorders.Count == 0)
                return (0.0, 0);

            // Search for the first item, which half is lower than mouse
            int index = 0;
            while (index < dropEntryBorders.Count && localPosition.Y > (dropEntryBorders[index].yStart + dropEntryBorders[index].height / 2.0))
                index++;

            // Is mouse below last element?
            if (index == dropEntryBorders.Count)
            {
                return (dropEntryBorders.Last().yStart + dropEntryBorders.Last().height, dropEntryBorders.Count);
            }
            else
            {
                return (dropEntryBorders[index].yStart, index);
            }
        }

        private void ColumnDragEnter(object sender, DragEventArgs e)
        {
            var entryViewModel = e.Data.GetData(typeof(EntryViewModel));
            if (entryViewModel != null)
            {
                e.Handled = true;
                e.Effects = DragDropEffects.Move;

                var items = sender as ItemsControl;
                var position = e.GetPosition(items);

                EvalEntryBorders(items);
                (double adornerY, int newIndex) = EvalDropData(items, position);
                AddDropAdorner(items, adornerY);
            }
        }

        private void ColumnDragOver(object sender, DragEventArgs e)
        {
            var entryViewModel = e.Data.GetData(typeof(EntryViewModel));
            if (entryViewModel != null)
            {
                e.Handled = true;
                var items = sender as ItemsControl;
                var position = e.GetPosition(items);

                (double adornerY, int newIndex) = EvalDropData(items, position);
                dropAdorner.UpdatePosition(adornerY);
            }
        }

        private void ColumnDragLeave(object sender, DragEventArgs e)
        {
            var entryViewModel = e.Data.GetData(typeof(EntryViewModel));
            if (entryViewModel != null && dropAdorner != null)
            {
                e.Handled = true;

                var items = sender as ItemsControl;
                RemoveDropAdorner(items);
            }
        }

        private void ColumnDrop(object sender, DragEventArgs e)
        {
            var entryViewModel = (EntryViewModel)e.Data.GetData(typeof(EntryViewModel));
            if (entryViewModel != null && dropAdorner != null)
            {
                var items = sender as ItemsControl;
                var position = e.GetPosition(items);
                (double adornerY, int newIndex) = EvalDropData(items, position);

                viewModel.RequestMoveEntry(entryViewModel, newIndex);

                RemoveDropAdorner(items);
            }
        }
    }
}
