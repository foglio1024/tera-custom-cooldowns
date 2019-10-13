using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class ItemLevelTierToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object ret;
            if (targetType == typeof(Brush)) ret = R.Brushes.IceBrushLight;
            else ret = R.Colors.IceColorLight;

            if (value == null) return ret;

            var tierInt = (int)value;
            var tier = (ItemLevelTier)tierInt;

            if (targetType == typeof(Brushes))
                switch (tier)
                {
                    case ItemLevelTier.Tier0: ret = R.Brushes.IceBrushLight; break;
                    case ItemLevelTier.Tier1: ret = R.Brushes.Tier1DungeonBrush; break;
                    case ItemLevelTier.Tier2: ret = R.Brushes.Tier2DungeonBrush; break;
                    case ItemLevelTier.Tier3: ret = R.Brushes.Tier3DungeonBrush; break;
                    case ItemLevelTier.Tier4: ret = R.Brushes.Tier4DungeonBrush; break;
                    case ItemLevelTier.Tier5: ret = R.Brushes.Tier5DungeonBrush; break;
                    case ItemLevelTier.Tier6: ret = R.Brushes.HpBrush; break;
                    case ItemLevelTier.Tier7: ret = R.Brushes.AssaultStanceBrush; break;
                }
            else
                switch (tier)
                {
                    case ItemLevelTier.Tier0: ret = R.Colors.IceColorLight; break;
                    case ItemLevelTier.Tier1: ret = R.Colors.Tier1DungeonColor; break;
                    case ItemLevelTier.Tier2: ret = R.Colors.Tier2DungeonColor; break;
                    case ItemLevelTier.Tier3: ret = R.Colors.Tier3DungeonColor; break;
                    case ItemLevelTier.Tier4: ret = R.Colors.Tier4DungeonColor; break;
                    case ItemLevelTier.Tier5: ret = R.Colors.Tier5DungeonColor; break;
                    case ItemLevelTier.Tier6: ret = R.Colors.HpColor; break;
                    case ItemLevelTier.Tier7: ret = R.Colors.AssaultStanceColor; break;
                }

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class EntriesToColor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var retCol = Colors.Transparent;
            if (!(values[0] is int entries) || !(values[1] is int max)) { return targetType == typeof(Brush) ? new SolidColorBrush(retCol) : (object)retCol; }

            retCol = R.Colors.Tier3DungeonColor;
            if (entries == max) retCol = R.Colors.GreenColor;
            else if (entries == 0) retCol = R.Colors.AssaultStanceColor;

            return targetType == typeof(Brush) ? new SolidColorBrush(retCol) : (object)retCol;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
