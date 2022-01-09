using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Board.Common.Wpf.Helpers
{
    public static class ColorHelper
    {
        public static Color ToColor(this int i)
        {
            return Color.FromArgb((byte)(i >> 24 & 0xff), (byte)(i >> 16 & 0xff), (byte)(i >> 8 & 0xff), (byte)(i & 0xff));
        }

        public static int ToInt(this Color color)
        {
            return color.A << 24 | color.R << 16 | color.G << 8 | color.B;
        }
    }
}
