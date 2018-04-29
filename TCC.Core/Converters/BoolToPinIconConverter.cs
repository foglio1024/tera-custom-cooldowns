using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    internal class BoolToSvgSwitchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = new string[2];
            if (parameter != null) p = (parameter as string).Split(':');
            if ((bool)value) return App.Current.FindResource(p[0]) as Geometry;
            else return App.Current.FindResource(p[1]) as Geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
