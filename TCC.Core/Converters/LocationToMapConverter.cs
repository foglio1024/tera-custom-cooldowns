using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using TCC.Data.Map;

namespace TCC.Converters
{
    public class LocationToMapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var loc = (Location)value;
            return Path.GetDirectoryName(typeof(App).Assembly.Location)+ "/resources/images/maps/" + SessionManager.MapDatabase.GetMapId(loc.World, loc.Guard, loc.Section) + ".jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
