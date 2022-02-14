using Board.Common.Wpf.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Board.Converters
{
    public abstract class BaseIncomingDateConverter
    {
        protected readonly static Color criticalColor = Color.FromArgb(255, 255, 120, 120);
        protected readonly static Color mediumColor = Color.FromArgb(255, 255, 255, 170);
        protected readonly static Color lowColor = Color.FromArgb(255, 170, 255, 180);
        protected readonly static Color noColor = Color.FromArgb(0, 0, 0, 0);

        protected readonly static SolidColorBrush doneBrush = new SolidColorBrush(noColor);
        protected readonly static SolidColorBrush[] dayBrushes = new SolidColorBrush[15];

        static BaseIncomingDateConverter()
        {
            for (int i = 0; i < 7; i++)
            {
                dayBrushes[i] = new SolidColorBrush(criticalColor.GradientTo(mediumColor, (byte)(i * 100 / 7)));
            }
            for (int i = 8; i < 15; i++)
            {
                dayBrushes[i] = new SolidColorBrush(mediumColor.GradientTo(lowColor, (byte)((i - 7) * 100 / 7)));
            }
        }
    }
}
