using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class MoneyAmountToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            var amount = (long)(value ?? 0);
            return amount == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}