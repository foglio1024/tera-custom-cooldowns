using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class MoneyAmountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            var amount = (long)value;
            return amount == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}