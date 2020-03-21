using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class ClassToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Class?)value ?? Class.Common;
            var color = targetType == typeof(Color);


            return (c, color) switch
            {
                (Class.Lancer, true) => R.Colors.TankRoleColor,
                (Class.Lancer, false) => R.Brushes.TankRoleBrush,
                (Class.Brawler, true) => R.Colors.TankRoleColor,
                (Class.Brawler, false) => R.Brushes.TankRoleBrush,
                (Class.Priest, true) => R.Colors.HealerRoleColor,
                (Class.Priest, false) => R.Brushes.HealerRoleBrush,
                (Class.Mystic, true) => R.Colors.HealerRoleColor,
                (Class.Mystic, false) => R.Brushes.HealerRoleBrush,
                (_, true) => R.Colors.DpsRoleColor,
                (_, false) => R.Brushes.DpsRoleBrush
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}