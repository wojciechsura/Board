using Board.Common.Wpf.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Board.Common.Wpf.Converters
{
    public class IntToForegroundColorConverter : IValueConverter
    {
        private static readonly Color white = Color.FromArgb(255, 255, 255, 255);
        private static readonly Color black = Color.FromArgb(255, 0, 0, 0);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int iValue)
            {
                Color color = iValue.ToColor();
                int brightness = (color.R + color.G + color.B) / 3;

                if (brightness > 128)
                    return black;
                else
                    return white;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
