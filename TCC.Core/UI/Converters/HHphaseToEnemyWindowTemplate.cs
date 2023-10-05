using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TCC.R;

namespace TCC.UI.Converters;

public class HHphaseToEnemyWindowTemplate : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (HarrowholdPhase?) value switch
        {
            HarrowholdPhase.Phase1 => DataTemplates.Phase1EnemyWindowLayout,
            _ => DataTemplates.DefaultEnemyWindowLayout
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}