using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class SizeToStackLabelSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = (double)value;
            return size / 1.7;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
