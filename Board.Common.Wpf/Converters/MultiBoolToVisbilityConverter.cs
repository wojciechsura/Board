using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Board.Common.Wpf.Converters
{
    public class MultiBoolToVisbilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.All(v => v != null && v is bool))
            {
                var b = values.Cast<bool>().Aggregate(true, (current, next) => current && next);

                if (b)
                    return Visibility.Visible;
                else
                {
                    if (parameter is Visibility visibility)
                        return visibility;
                    else
                        return Visibility.Collapsed;
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
