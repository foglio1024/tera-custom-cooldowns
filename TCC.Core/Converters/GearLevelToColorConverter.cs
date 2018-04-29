using System;
using System.Globalization;
using System.Windows.Data;

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
                    return App.Current.FindResource("Tier2DungeonColor");
                case GearTier.Mid:
                    return App.Current.FindResource("Tier3DungeonColor");
                case GearTier.High:
                    return App.Current.FindResource("Tier4DungeonColor");
                case GearTier.Top:
                    return App.Current.FindResource("Tier5DungeonColor");
                default:
                    return App.Current.FindResource("TierSoloDungeonColor");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
