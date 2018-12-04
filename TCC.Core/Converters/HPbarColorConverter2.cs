using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class HPbarColorConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?) value ?? false
                ? R.Colors.HpDebuffColorLight //Application.Current.FindResource("HpDebuffColorLight")
                : R.Colors.HpColorLight; //Application.Current.FindResource("HpColorLight");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}