using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class GearLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (GearTier? ) value ?? GearTier.Low;
            switch (val)
            {
                case GearTier.Low:
                    return R.Brushes.Tier2DungeonBrush; //Application.Current.FindResource("Tier2DungeonBrush");
                case GearTier.Mid:
                    return R.Brushes.Tier3DungeonBrush; //Application.Current.FindResource("Tier3DungeonBrush");
                case GearTier.High:
                    return R.Brushes.Tier4DungeonBrush;//Application.Current.FindResource("Tier4DungeonBrush");
                case GearTier.Top:
                    return R.Brushes.Tier5DungeonBrush; //Application.Current.FindResource("Tier5DungeonBrush");
                default:
                    return R.Brushes.TierSoloDungeonBrush; // Application.Current.FindResource("TierSoloDungeonBrush");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
