using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class MessageTextToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var icConv = new IconConverter();
            var val = value?.ToString();
            return icConv.Convert(val, targetType, parameter, culture);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
