using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TCC.Data;
using Brushes = TCC.R.Brushes;

namespace TCC.UI.Converters;

public class WarriorStanceToColorConverter : MarkupExtension, IValueConverter
{
    public bool Light { get; set; }
    public bool Fallback { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var col = targetType == typeof(Color);
        var stance = (WarriorStance?)value ?? WarriorStance.None;

        var lightFback = Colors.DarkGray;
        lightFback.A = 170;

        return (stance, col, Light) switch
        {
            (WarriorStance.Assault, false, false) => Brushes.AssaultStanceBrush,
            (WarriorStance.Assault, true, false) => R.Colors.AssaultStanceColor,
            (WarriorStance.Assault, false, true) => Brushes.AssaultStanceBrushLight,
            (WarriorStance.Assault, true, true) => R.Colors.AssaultStanceColorLight,
            (WarriorStance.Defensive, false, false) => Brushes.DefensiveStanceBrush,
            (WarriorStance.Defensive, true, false) => R.Colors.DefensiveStanceColor,
            (WarriorStance.Defensive, false, true) => Brushes.DefensiveStanceBrushLight,
            (WarriorStance.Defensive, true, true) => R.Colors.DefensiveStanceColorLight,
            (WarriorStance.None, false, false) => new SolidColorBrush(lightFback),
            (WarriorStance.None, true, false) => lightFback,
            (WarriorStance.None, false, true) => System.Windows.Media.Brushes.White,
            (WarriorStance.None, true, true) => Colors.White,
            _ => null
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}