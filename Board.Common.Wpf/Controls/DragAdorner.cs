using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Board.Common.Wpf.Controls
{
    public class DragAdorner : Adorner
    {
        private Point location;
        private Point offset;
        private Brush vbrush;

        protected override void OnRender(DrawingContext dc)
        {
            var p = location;
            p.Offset(-offset.X, -offset.Y);

            dc.DrawRectangle(vbrush, null, new Rect(p, this.RenderSize));
        }

        public DragAdorner(UIElement adornedElement, Point offset)
                                            : base(adornedElement)
        {
            this.offset = offset;
            IsHitTestVisible = false;

            vbrush = new VisualBrush(AdornedElement);
            vbrush.Opacity = .7;
        }

        public void UpdatePosition(Point location)
        {
            this.location = location;
            this.InvalidateVisual();
        }
    }
}
