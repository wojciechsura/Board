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

        public static Color GradientTo(this Color from, Color to, byte step)
        {
            return Color.FromArgb((byte)(from.A + (to.A - from.A) * step / 100),
                (byte)(from.R + (to.R - from.R) * step / 100),
                (byte)(from.G + (to.G - from.G) * step / 100),
                (byte)(from.B + (to.B - from.B) * step / 100));
        }

        public static byte GetLuminance(this Color color)
        {
            byte max = Math.Max(color.R, Math.Max(color.G, color.B));
            byte min = Math.Min(color.R, Math.Min(color.G, color.B));
            return (byte)((max + min) / 2);
        }
    }
}
