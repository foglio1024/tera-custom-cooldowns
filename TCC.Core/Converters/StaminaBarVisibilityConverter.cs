using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.Converters
{
    public class StaminaBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Class cl)) cl = Class.None;

            return cl == Class.Warrior || cl == Class.Lancer || cl == Class.Archer
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
