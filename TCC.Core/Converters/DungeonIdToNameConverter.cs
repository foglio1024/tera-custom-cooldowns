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
                if (DungeonDatabase.Instance.DungeonDefinitions.ContainsKey(dgId)) return DungeonDatabase.Instance.DungeonDefinitions[dgId].ShortName;
            }
            if (DungeonDatabase.Instance.DungeonNames.ContainsKey(dgId)) return DungeonDatabase.Instance.DungeonNames[dgId];
            else return "Dungeon "+ dgId.ToString(); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
