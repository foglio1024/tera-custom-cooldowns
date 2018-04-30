using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.Converters
{
    internal class LocationToMarkerPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var loc = (Location)value;
            var point = SessionManager.MapDatabase.GetMarkerPosition(loc);
            Console.WriteLine($"{point.X},{point.Y}");
            return new Thickness(point.Y,point.X, 0, 0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
