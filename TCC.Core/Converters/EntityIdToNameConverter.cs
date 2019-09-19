using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class EntityIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (ulong) value;
            return Game.IsMe(id)
                ? Game.Me.Name
                : EntityManager.IsEntitySpawned(id) 
                    ? EntityManager.GetEntityName(id) 
                    : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}