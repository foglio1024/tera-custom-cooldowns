using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class LocationToMapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var loc = (Location)value;
            return AppDomain.CurrentDomain.BaseDirectory + "/resources/images/maps/" + SessionManager.MapDatabase.GetMapId(loc.World, loc.Guard, loc.Section) + ".jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
