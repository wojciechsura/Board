using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Board.Converters
{
    public class CommentButtonsVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {           
            if (values[0] is string || values[0] == null)
                if (values[1] is bool isNew)
                {
                    var content = values[0] as string;

                    if (!isNew || !string.IsNullOrEmpty(content))
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
