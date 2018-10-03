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
            var val = (GearTier? ) value ?? GearTier.Low;
            switch (val)
            {
                case GearTier.Low:
                    return Application.Current.FindResource("Tier2DungeonBrush");
                case GearTier.Mid:
                    return Application.Current.FindResource("Tier3DungeonBrush");
                case GearTier.High:
                    return Application.Current.FindResource("Tier4DungeonBrush");
                case GearTier.Top:
                    return Application.Current.FindResource("Tier5DungeonBrush");
                default:
                    return Application.Current.FindResource("TierSoloDungeonBrush");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
