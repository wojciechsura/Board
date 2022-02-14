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
    public class IncomingDateToBackgroundConverter : BaseIncomingDateConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                if (values[0] is DateTime date && values[1] is bool done)
                {
                    if (done)
                        return doneBrush;

                    var diff = date - DateTime.Now;

                    var days = Math.Max(0, diff.Days);
                    if (days > 14)
                        return doneBrush;
                    else
                        return dayBrushes[days];
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
