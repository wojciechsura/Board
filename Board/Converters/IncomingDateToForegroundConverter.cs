using Board.Common.Wpf.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Board.Converters
{
    public class IncomingDateToForegroundConverter : BaseIncomingDateConverter, IMultiValueConverter
    {
        private static Brush lightBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private static Brush darkBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                // No date
                if (values[0] == null)
                    return darkBrush;

                // Date and isDone
                if (values[0] is DateTime date && values[1] is bool done)
                {
                    if (done)
                        return darkBrush;

                    var diff = date - DateTime.Now;

                    var days = Math.Max(0, diff.Days);
                    if (days > 14)
                        return darkBrush;
                    else
                    {
                        var backBrush = dayBrushes[days];
                        var luminance = backBrush.Color.GetLuminance();

                        if (luminance > 200)
                            return darkBrush;
                        else
                            return lightBrush;
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
