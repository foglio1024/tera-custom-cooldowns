using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    public class WinningToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO: to triggers btw
            return (bool?) value ?? false
                ? new SolidColorBrush(Color.FromRgb(0xff, 0xcc, 0x00)) //TODO: to resource
                : System.Windows.Application.Current.FindResource("IgnidraxColor"); //TODO: check color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
