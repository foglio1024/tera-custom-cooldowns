using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    internal class GearLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (GearTier) value;
            switch (val)
            {
                case GearTier.Low:
                    return Application.Current.FindResource("Tier2DungeonColor");
                case GearTier.Mid:
                    return Application.Current.FindResource("Tier3DungeonColor");
                case GearTier.High:
                    return Application.Current.FindResource("Tier4DungeonColor");
                case GearTier.Top:
                    return Application.Current.FindResource("Tier5DungeonColor");
                default:
                    return Application.Current.FindResource("TierSoloDungeonColor");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
