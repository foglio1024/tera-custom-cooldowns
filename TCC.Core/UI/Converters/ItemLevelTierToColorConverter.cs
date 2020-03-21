using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;
using TCC.Utils;

namespace TCC.UI.Converters
{
    public class ItemLevelTierToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object ret;
            if (targetType == typeof(Brush)) ret = R.Brushes.IceBrushLight;
            else ret = R.Colors.IceColorLight;

            if (value == null) return ret;

            try
            {
                var tierInt = (int)value;
                var tier = (ItemLevelTier)tierInt;

                if (targetType == typeof(Brush))
                    ret = tier switch
                    {
                        ItemLevelTier.Tier0 => R.Brushes.IceBrushLight,
                        ItemLevelTier.Tier1 => R.Brushes.Tier1DungeonBrush,
                        ItemLevelTier.Tier2 => R.Brushes.Tier2DungeonBrush,
                        ItemLevelTier.Tier3 => R.Brushes.Tier3DungeonBrush,
                        ItemLevelTier.Tier4 => R.Brushes.Tier4DungeonBrush,
                        ItemLevelTier.Tier5 => R.Brushes.Tier5DungeonBrush,
                        ItemLevelTier.Tier6 => R.Brushes.HpBrush,
                        ItemLevelTier.Tier7 => R.Brushes.AssaultStanceBrush,
                        _ => ret
                    };
                else
                    ret = tier switch
                    {
                        ItemLevelTier.Tier0 => R.Colors.IceColorLight,
                        ItemLevelTier.Tier1 => R.Colors.Tier1DungeonColor,
                        ItemLevelTier.Tier2 => R.Colors.Tier2DungeonColor,
                        ItemLevelTier.Tier3 => R.Colors.Tier3DungeonColor,
                        ItemLevelTier.Tier4 => R.Colors.Tier4DungeonColor,
                        ItemLevelTier.Tier5 => R.Colors.Tier5DungeonColor,
                        ItemLevelTier.Tier6 => R.Colors.HpColor,
                        ItemLevelTier.Tier7 => R.Colors.AssaultStanceColor,
                        _ => ret
                    };
            }
            catch(Exception e)
            {
                Log.F($"Failed to convert ilvl to color {e}");
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
