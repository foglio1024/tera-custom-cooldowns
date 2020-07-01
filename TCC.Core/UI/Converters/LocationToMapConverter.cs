using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using TCC.Data.Map;

namespace TCC.UI.Converters
{
    public class LocationToMapConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var loc = (Location)value;
            return Path.Combine(App.ResourcesPath, "images/maps/" + Game.DB.MapDatabase.GetMapId(loc.World, loc.Guard, loc.Section) + ".jpg");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
