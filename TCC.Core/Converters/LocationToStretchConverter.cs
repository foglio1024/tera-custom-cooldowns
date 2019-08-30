using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data.Map;

namespace TCC.Converters
{
    public class LocationToStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var loc = (Location)value;
            if (Game.DB.MapDatabase.IsDungeon(loc))
            {
                return Stretch.Uniform;
            }
            else
            {
                return Stretch.UniformToFill;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
