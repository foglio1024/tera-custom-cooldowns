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
            var retCol = R.Colors.IceColorLight;
            if (value is ItemLevelTier tier)
            {
                switch (tier)
                {
                    case ItemLevelTier.Tier0:
                        retCol = R.Colors.IceColorLight;
                        break;
                    case ItemLevelTier.Tier1:
                        retCol = R.Colors.Tier1DungeonColor;
                        break;
                    case ItemLevelTier.Tier2:
                        retCol = R.Colors.Tier2DungeonColor;
                        break;
                    case ItemLevelTier.Tier3:
                        retCol = R.Colors.Tier3DungeonColor;
                        break;
                    case ItemLevelTier.Tier4:
                        retCol = R.Colors.Tier4DungeonColor;
                        break;
                    case ItemLevelTier.Tier5:
                        retCol = R.Colors.Tier5DungeonColor;
                        break;
                    case ItemLevelTier.Tier6:
                        retCol = R.Colors.HpColor;
                        break;
                    case ItemLevelTier.Tier7:
                        retCol = R.Colors.AssaultStanceColor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (targetType == typeof(Brush)) return new SolidColorBrush(retCol);
            return retCol;
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
            if(!(values[0] is int entries) || !(values[1] is int max)) { return targetType == typeof(Brush) ? new SolidColorBrush(retCol) : (object)retCol; }

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
