using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Utilities;

namespace TCC.UI.Converters
{
    public class EntityIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var id = (ulong)value;
            return Game.IsMe(id)
                ? Game.Me.Name
                : TccUtils.IsEntitySpawned(id)
                    ? TccUtils.GetEntityName(id)
                    : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}