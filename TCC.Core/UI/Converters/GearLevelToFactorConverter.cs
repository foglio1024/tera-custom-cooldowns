using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TeraDataLite;

namespace TCC.UI.Converters;

public class GearLevelToFactorConverter : IValueConverter
{
    const int Levels = 37;
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not GearItem val) return 0;
        var t = 0;
        if (val.Tier > GearTier.Low)
        {
            t = ((int) val.Tier - 1) * 9 + 6;
        }
        var level = t + val.Enchant;
        return level / (double)Levels;

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}