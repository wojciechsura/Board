using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Board.Common.Wpf.Converters
{
    public class StreamToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MemoryStream ms)
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                imageSource.StreamSource = ms;
                imageSource.EndInit();

                return imageSource;
            }
            else if (value == null)
                return null;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
