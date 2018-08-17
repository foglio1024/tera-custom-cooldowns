using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.Converters
{
    public class BoolToVisibleCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inv = false;
            if (parameter != null)
            {
                if (bool.Parse(parameter.ToString()))
                {
                    inv = true;
                }
            }
            if (!(value is bool)) return Visibility.Visible;
            var val = (bool) value;
            if (inv) val = !val;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
