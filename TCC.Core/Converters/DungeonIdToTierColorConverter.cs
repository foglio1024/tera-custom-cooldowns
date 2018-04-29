using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data.Databases;

namespace TCC.Converters
{
    internal class DungeonIdToTierColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (uint)value;
            var t = DungeonDatabase.Instance.Dungeons[id].Tier;
            switch (t)
            {
                case DungeonTier.Tier2:
                    return App.Current.FindResource("Tier2DungeonColor");
                case DungeonTier.Tier3:
                    return App.Current.FindResource("Tier3DungeonColor");
                case DungeonTier.Tier4:
                    return App.Current.FindResource("Tier4DungeonColor");
                case DungeonTier.Tier5:
                    return App.Current.FindResource("Tier5DungeonColor");
                default:
                    return App.Current.FindResource("TierSoloDungeonColor");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
