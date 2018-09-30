using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.Converters
{
    public class HPbarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?) value ?? false
                ? Application.Current.FindResource("HpDebuffColorBase")
                : Application.Current.FindResource("HpColorBase");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
