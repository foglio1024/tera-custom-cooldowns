using System;
using System.Globalization;
using System.Windows.Data;
using TCC.R;
using TeraDataLite;

namespace TCC.UI.Converters;

public class GearLevelToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var val = (GearTier? ) value ?? GearTier.Low;
        return val switch
        {
            GearTier.Low => Brushes.Tier2DungeonBrush,
            GearTier.Mid => Brushes.Tier3DungeonBrush,
            GearTier.High => Brushes.Tier4DungeonBrush,
            GearTier.Top => Brushes.Tier5DungeonBrush,
            _ => Brushes.Tier1DungeonBrush
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}