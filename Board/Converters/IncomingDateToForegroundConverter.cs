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
    public class IncomingDateToForegroundConverter : IValueConverter
    {
        private static Brush notMuchTimeBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private static Brush aLotOfTimeBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                var diff = date - DateTime.Now;
                
                if (diff.Days < 1)
                    return notMuchTimeBrush;
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
