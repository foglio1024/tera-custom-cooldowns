using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;
using TCC.Utils;
using Brushes = TCC.R.Brushes;
using Colors = TCC.R.Colors;

namespace TCC.UI.Converters;

public class ItemLevelTierToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        object ret;
        if (targetType == typeof(Brush)) ret = Brushes.IceBrushLight;
        else ret = Colors.IceColorLight;

        if (value == null) return ret;

        try
        {
            var tierInt = (int)value;
            var tier = (ItemLevelTier)tierInt;

            if (targetType == typeof(Brush))
                ret = tier switch
                {
                    ItemLevelTier.Tier0 => Brushes.IceBrushLight,
                    ItemLevelTier.Tier1 => Brushes.Tier1DungeonBrush,
                    ItemLevelTier.Tier2 => Brushes.Tier2DungeonBrush,
                    ItemLevelTier.Tier3 => Brushes.Tier3DungeonBrush,
                    ItemLevelTier.Tier4 => Brushes.Tier4DungeonBrush,
                    ItemLevelTier.Tier5 => Brushes.Tier5DungeonBrush,
                    ItemLevelTier.Tier6 => Brushes.HpBrush,
                    ItemLevelTier.Tier7 => Brushes.AssaultStanceBrush,
                    _ => ret
                };
            else
                ret = tier switch
                {
                    ItemLevelTier.Tier0 => Colors.IceColorLight,
                    ItemLevelTier.Tier1 => Colors.Tier1DungeonColor,
                    ItemLevelTier.Tier2 => Colors.Tier2DungeonColor,
                    ItemLevelTier.Tier3 => Colors.Tier3DungeonColor,
                    ItemLevelTier.Tier4 => Colors.Tier4DungeonColor,
                    ItemLevelTier.Tier5 => Colors.Tier5DungeonColor,
                    ItemLevelTier.Tier6 => Colors.HpColor,
                    ItemLevelTier.Tier7 => Colors.AssaultStanceColor,
                    _ => ret
                };
        }
        catch(Exception e)
        {
            Log.F($"Failed to convert ilvl to color {e}");
        }
        return ret;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}