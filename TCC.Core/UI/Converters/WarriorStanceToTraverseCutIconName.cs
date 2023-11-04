using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.UI.Converters;

public class WarriorStanceToTraverseCutIconName : IValueConverter
{
    public string? AssaultStanceIconName { get; set; }
    public string? DefensiveStanceIconName { get; set; }
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not WarriorStance stance) return null;

        return stance switch
        {
            WarriorStance.Assault => AssaultStanceIconName,
            WarriorStance.Defensive => DefensiveStanceIconName,
            WarriorStance.None => AssaultStanceIconName,
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}