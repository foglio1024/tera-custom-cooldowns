using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;
using Colors = TCC.R.Colors;
using Brushes = TCC.R.Brushes;


namespace TCC.UI.Converters;

public class ClassToFillConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var c = (Class?)value ?? Class.Common;
        var color = targetType == typeof(Color);


        return (c, color) switch
        {
            (Class.Lancer, true) => Colors.TankRoleColor,
            (Class.Lancer, false) => Brushes.TankRoleBrush,
            (Class.Brawler, true) => Colors.TankRoleColor,
            (Class.Brawler, false) => Brushes.TankRoleBrush,
            (Class.Priest, true) => Colors.HealerRoleColor,
            (Class.Priest, false) => Brushes.HealerRoleBrush,
            (Class.Mystic, true) => Colors.HealerRoleColor,
            (Class.Mystic, false) => Brushes.HealerRoleBrush,
            (_, true) => Colors.DpsRoleColor,
            (_, false) => Brushes.DpsRoleBrush
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}