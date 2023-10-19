using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;

namespace TCC.UI.Converters;

public class CooldownWindowModeToTemplateConverter : IValueConverter
{
    public DataTemplate? Fixed { get; set; }
    public DataTemplate? Normal { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CooldownBarMode cbm) return null;
        return cbm switch
        {
            CooldownBarMode.Fixed => Fixed,
            CooldownBarMode.Normal => Normal,
            _ => null
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}