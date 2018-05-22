using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.Converters
{
    internal class ContractedToColWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            if (!b)
            {
                return new GridLength(1, GridUnitType.Star);
            }
            else
            {
                return new GridLength(1, GridUnitType.Auto);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
