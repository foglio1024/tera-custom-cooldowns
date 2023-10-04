using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.UI.Converters;

public class StaminaBarVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Class cl) cl = Class.None;

        return cl is Class.Warrior or Class.Lancer or Class.Archer
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}