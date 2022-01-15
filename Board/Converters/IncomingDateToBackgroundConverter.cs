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
    public class IncomingDateToBackgroundConverter : IValueConverter
    {
        private static Brush lessThanADayBrush = new SolidColorBrush(Color.FromArgb(255, 255, 128, 102));
        private static Brush lessThanThreeDaysBrush = new SolidColorBrush(Color.FromArgb(255, 255, 108, 0));
        private static Brush lessThanWeekBrush = new SolidColorBrush(Color.FromArgb(255, 255, 210, 0));
        private static Brush aLotOfTimeBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
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

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
