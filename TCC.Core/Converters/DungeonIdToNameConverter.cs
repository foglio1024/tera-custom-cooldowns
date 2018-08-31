using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    internal class DungeonIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dgId = (uint)value;
            if (parameter != null && bool.Parse((string)parameter))
            {
                //use short names
                if (SessionManager.DungeonDatabase.DungeonDefs.ContainsKey(dgId)) return SessionManager.DungeonDatabase.DungeonDefs[dgId].Name;//.DungeonDefinitions[dgId].ShortName;
            }
            if (SessionManager.DungeonDatabase.DungeonDefs.ContainsKey(dgId)) return SessionManager.DungeonDatabase.DungeonDefs[dgId].Name;
            else return "Dungeon "+ dgId.ToString(); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
