using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.Converters
{
    public class DurationToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var duration = (uint)value;
            if (duration == uint.MaxValue)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
