using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class ShieldStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ShieldStatus?)value) //TODO: triggers?
            {
                case ShieldStatus.Off:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }

        }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}