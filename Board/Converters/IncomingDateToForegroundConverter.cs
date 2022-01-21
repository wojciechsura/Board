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
    public class IncomingDateToForegroundConverter : IMultiValueConverter
    {
        private static Brush notMuchTimeBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private static Brush aLotOfTimeBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private static Brush doneBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                // No date
                if (values[0] == null)
                    return aLotOfTimeBrush;

                // Date and isDone
                if (values[0] is DateTime date && values[1] is bool done)
                {
                    if (done)
                        return doneBrush;

                    var diff = date - DateTime.Now;

                    if (diff.Days < 1)
                        return notMuchTimeBrush;
                    else
                        return aLotOfTimeBrush;
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
