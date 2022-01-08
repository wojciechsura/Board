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
    /// Interaction logic for Table.xaml
    /// </summary>
    public partial class Table : ScrollViewer
    {
        private const double ADORNER_WIDTH = 5;
        private readonly Point POINT_ZERO = new Point(0, 0);

        private class ColumnDropAdorner : Adorner
        {
            private Brush brush;
            private double locationX;

            public ColumnDropAdorner(UIElement adornedElement, double locationX)
                : base(adornedElement)
            {
                brush = new SolidColorBrush(Color.FromArgb(255, 30, 60, 90));
                this.locationX = locationX;
                IsHitTestVisible = false;
            }

            public void UpdatePosition(double locationX)
            {
                this.locationX = locationX;
                InvalidateVisual();
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRectangle(brush, null, new Rect(locationX - ADORNER_WIDTH / 2, 0, ADORNER_WIDTH, this.RenderSize.Height));
            }
        }

        private ColumnDropAdorner dropAdorner;
        private List<(double xStart, double width)> dropColumnBorders;
        private TableViewModel viewModel;

        private void AddDropAdorner(ItemsControl items, double locationX)
        {
            dropAdorner = new ColumnDropAdorner(items, locationX);
            var layer = AdornerLayer.GetAdornerLayer(items);
            layer.Add(dropAdorner);
        }

        private void RemoveDropAdorner(ItemsControl items)
        {
            var layer = AdornerLayer.GetAdornerLayer(items);
            layer.Remove(dropAdorner);
            dropAdorner = null;
        }

        private void EvalColumnBorders(ItemsControl items)
        {
            dropColumnBorders = new List<(double xStart, double width)>();

            for (int i = 0; i < items.Items.Count; i++)
            {
                FrameworkElement item = (FrameworkElement)items.ItemContainerGenerator.ContainerFromIndex(i);
                var topLeft = item.TranslatePoint(POINT_ZERO, items);

                dropColumnBorders.Add((topLeft.X, item.ActualWidth));
            }
        }

        private (double adornerX, int newIndex) EvalDropData(ItemsControl items, Point localPosition)
        {
            if (dropColumnBorders.Count == 0)
                return (0.0, 0);

            // Search for the first item, which half is further than mouse
            int index = 0;
            while (index < dropColumnBorders.Count && localPosition.X > (dropColumnBorders[index].xStart + dropColumnBorders[index].width / 2.0))
                index++;

            // Is mouse below last element?
            if (index == dropColumnBorders.Count)
            {
                return (dropColumnBorders.Last().xStart + dropColumnBorders.Last().width, dropColumnBorders.Count);
            }
            else
            {
                return (dropColumnBorders[index].xStart, index);
            }
        }

        public Table()
        {
            InitializeComponent();
        }

        private void TableDragEnter(object sender, DragEventArgs e)
        {
            var columnViewModel = e.Data.GetData(typeof(ColumnViewModel));
            if (columnViewModel != null)
            {
                e.Handled = true;
                e.Effects = DragDropEffects.Move;

                var items = sender as ItemsControl;
                var position = e.GetPosition(items);

                EvalColumnBorders(items);
                (double adornerX, int newIndex) = EvalDropData(items, position);
                AddDropAdorner(items, adornerX);
            }
        }

        private void TableDragOver(object sender, DragEventArgs e)
        {
            var columnViewModel = e.Data.GetData(typeof(ColumnViewModel));
            if (columnViewModel != null)
            {
                e.Handled = true;
                var items = sender as ItemsControl;
                var position = e.GetPosition(items);

                (double adornerX, int newIndex) = EvalDropData(items, position);
                dropAdorner.UpdatePosition(adornerX);
            }
        }

        private void TableDragLeave(object sender, DragEventArgs e)
        {
            var columnViewModel = e.Data.GetData(typeof(ColumnViewModel));
            if (columnViewModel != null && dropAdorner != null)
            {
                e.Handled = true;

                var items = sender as ItemsControl;
                RemoveDropAdorner(items);
            }
        }

        private void TableDrop(object sender, DragEventArgs e)
        {
            var columnViewModel = (ColumnViewModel)e.Data.GetData(typeof(ColumnViewModel));
            if (columnViewModel != null && dropAdorner != null)
            {
                var items = sender as ItemsControl;
                var position = e.GetPosition(items);
                (double adornerX, int newIndex) = EvalDropData(items, position);

                viewModel.RequestMoveColumn(columnViewModel, newIndex);                

                RemoveDropAdorner(items);
            }
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (TableViewModel)e.NewValue;
        }
    }
}
