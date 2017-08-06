using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TCC.Data.Databases;

namespace TCC.Converters
{
    class DungeonIdToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (uint)value;
            var t =  DungeonDatabase.Instance.DungeonDefinitions[id].Tier;
            switch (t)
            {
                case DungeonTier.Tier2:
                    return "2";
                case DungeonTier.Tier3:
                    return "3";
                case DungeonTier.Tier4:
                    return "4";
                case DungeonTier.Tier5:
                    return "5";
                default:
                    return "-";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
