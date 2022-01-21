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
    public class IncomingDateToBackgroundConverter : IMultiValueConverter
    {
        private static Brush lessThanADayBrush = new SolidColorBrush(Color.FromArgb(255, 230, 110, 110));
        private static Brush lessThanThreeDaysBrush = new SolidColorBrush(Color.FromArgb(255, 255, 228, 192));
        private static Brush lessThanWeekBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 192));
        private static Brush aLotOfTimeBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        private static Brush doneBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                if (values[0] is DateTime date && values[1] is bool done)
                {
                    if (done)
                        return doneBrush;

                    var diff = date - DateTime.Now;

                    if (diff.Days < 1)
                        return lessThanADayBrush;
                    if (diff.Days < 3)
                        return lessThanThreeDaysBrush;
                    if (diff.Days < 7)
                        return lessThanWeekBrush;
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
