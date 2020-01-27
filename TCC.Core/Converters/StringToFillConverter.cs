using Nostrum;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    public class StringToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (string)value;
            if (string.IsNullOrEmpty(c)) return R.Brushes.ChatSystemGenericBrush;
            if(targetType ==typeof(Brush)) return new SolidColorBrush(MiscUtils.ParseColor(c));
            if (targetType == typeof(Color)) return MiscUtils.ParseColor(c);
            return R.Brushes.ChatSystemGenericBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
