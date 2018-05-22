using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.Converters
{
    public class LocationToStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var loc = (Location)value;
            if (SessionManager.MapDatabase.GetDungeon(loc))
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
