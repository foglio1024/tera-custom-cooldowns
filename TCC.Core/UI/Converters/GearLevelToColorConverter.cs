using System;
using System.Globalization;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class GearLevelToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (GearTier? ) value ?? GearTier.Low;
            return val switch
            {
                GearTier.Low => R.Brushes.Tier2DungeonBrush,
                GearTier.Mid => R.Brushes.Tier3DungeonBrush,
                GearTier.High => R.Brushes.Tier4DungeonBrush,
                GearTier.Top => R.Brushes.Tier5DungeonBrush,
                _ => R.Brushes.Tier1DungeonBrush
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
