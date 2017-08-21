using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data.Databases;

namespace TCC.Converters
{
    class DungeonIdToTierColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (uint)value;
            var t = DungeonDatabase.Instance.DungeonDefinitions[id].Tier;
            switch (t)
            {
                case DungeonTier.Tier2:
                    return App.Current.FindResource("Colors.DungeonTier.2");
                case DungeonTier.Tier3:
                    return App.Current.FindResource("Colors.DungeonTier.3");
                case DungeonTier.Tier4:
                    return App.Current.FindResource("Colors.DungeonTier.4");
                case DungeonTier.Tier5:
                    return App.Current.FindResource("Colors.DungeonTier.5");
                default:
                    return App.Current.FindResource("Colors.DungeonTier.Solo");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
