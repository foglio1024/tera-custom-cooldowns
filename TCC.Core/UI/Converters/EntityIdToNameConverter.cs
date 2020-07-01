using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Utilities;
using TeraDataLite;

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
    public class EntityIdToClassConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var id = (ulong)value;
            var cl = Game.IsMe(id)
                ? Game.Me.Class
                : TccUtils.IsEntitySpawned(id)
                    ? TccUtils.GetEntityClass(id)
                    : Class.None;
            if (cl == Class.None) return null;
            return TccUtils.SvgClass(cl);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}