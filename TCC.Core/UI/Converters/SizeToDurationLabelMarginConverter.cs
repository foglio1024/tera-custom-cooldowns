using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.UI.Converters;

public class SizeToDurationLabelMarginConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var size = (double?)value ?? 0;
        return new Thickness(0, 0, 0, -size * 1.25);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}