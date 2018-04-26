using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data.Databases;

namespace TCC.Converters
{
    class DungeonIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dgId = (uint)value;
            if (parameter != null && Boolean.Parse((string)parameter))
            {
                //use short names
                if (DungeonDatabase.Instance.Dungeons.ContainsKey(dgId)) return DungeonDatabase.Instance.Dungeons[dgId].Name;//.DungeonDefinitions[dgId].ShortName;
            }
            if (DungeonDatabase.Instance.Dungeons.ContainsKey(dgId)) return DungeonDatabase.Instance.Dungeons[dgId].Name;
            else return "Dungeon "+ dgId.ToString(); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
