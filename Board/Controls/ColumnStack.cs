using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Board.Controls
{
    public class ColumnStack : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Size maxSize = new Size(Double.PositiveInfinity, availableSize.Height);

        var sumWidths = 0.0;

            foreach (FrameworkElement child in InternalChildren)
            {
                Size availableChildSize = new Size(Double.PositiveInfinity, availableSize.Height - child.Margin.Top - child.Margin.Bottom);
                child.Measure(availableChildSize);
                sumWidths += child.Margin.Left + child.DesiredSize.Width + child.Margin.Right;
            }

            return new Size(sumWidths, availableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var currentX = 0.0;

            foreach (FrameworkElement child in InternalChildren)
            {
                Point position = new Point(currentX + child.Margin.Left, child.Margin.Top);
                double maxChildHeight = finalSize.Height - child.Margin.Top - child.Margin.Bottom;
                double finalChildHeight = Math.Min(maxChildHeight, child.DesiredSize.Height);
                Size size = new Size(child.DesiredSize.Width, finalChildHeight);

                child.Arrange(new Rect(position, size));

                currentX += child.Margin.Left + child.DesiredSize.Width + child.Margin.Right;
            }

            return finalSize;
        }
    }
}
