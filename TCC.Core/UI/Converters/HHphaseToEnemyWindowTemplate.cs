using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.UI.Converters;

public class HHphaseToEnemyWindowTemplate : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (HarrowholdPhase?) value switch
        {
            HarrowholdPhase.Phase1 => R.DataTemplates.Phase1EnemyWindowLayout,
            _ => R.DataTemplates.DefaultEnemyWindowLayout
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}