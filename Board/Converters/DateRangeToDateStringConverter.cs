using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Board.Converters
{
    public class DateRangeToDateStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                if (values[0] == null && values[1] == null)
                {
                    return null;
                }
                else if (values[0] is DateTime startDate && values[1] == null)
                {
                    return $"{startDate.ToString("dd MMM yyyy")} - ...";
                }
                else if (values[0] == null && values[1] is DateTime endDate)
                {
                    return endDate.ToString("dd MMM yyyy");
                }
                else if (values[0] is DateTime startDate1 && values[1] is DateTime endDate1)
                {
                    if (startDate1.Year == endDate1.Year)
                    {
                        if (startDate1.Month == endDate1.Month)
                            return $"{startDate1:dd} - {endDate1:dd MMM yyyy}";
                        else
                            return $"{startDate1:dd MMM} - {endDate1:dd MMM yyyy}";
                    }
                    else
                        return $"{startDate1:dd MMM yyyy} - {endDate1:dd MMM yyyy}";
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
