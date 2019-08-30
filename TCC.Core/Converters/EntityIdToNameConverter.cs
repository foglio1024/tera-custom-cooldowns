using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class EntityIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            return Game.IsMe((ulong)value)
                ? Game.Me.Name
                : EntityManager.IsEntitySpawned((ulong)value) ? EntityManager.GetEntityName((ulong)value) /*(WindowManager.ViewModels.Group.TryGetUser((ulong) value, out var p) ? p.Name*/ : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}