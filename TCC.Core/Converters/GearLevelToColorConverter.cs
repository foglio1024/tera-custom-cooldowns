using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    class GearLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (GearTier) value;
            switch (val)
            {
                case GearTier.Low:
                    return App.Current.FindResource("Colors.DungeonTier.2");
                case GearTier.Mid:
                    return App.Current.FindResource("Colors.DungeonTier.3");
                case GearTier.High:
                    return App.Current.FindResource("Colors.DungeonTier.4");
                case GearTier.Top:
                    return App.Current.FindResource("Colors.DungeonTier.5");
                default:
                    return App.Current.FindResource("Colors.DungeonTier.Solo");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
