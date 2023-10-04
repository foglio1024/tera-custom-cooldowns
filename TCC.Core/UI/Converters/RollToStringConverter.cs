using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.UI.Converters;

public class RollToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (int?) value switch
        {
            -1 => "x",
            0 => "-",
            _ => ((int?) value).ToString()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}