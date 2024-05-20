using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TCC.UI.Converters;

public class IconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var iconName = value?.ToString()?.Replace(".", "/") ?? "unknown";

        var path = Path.Combine(App.ResourcesPath, "images/" + iconName + ".png");

        return File.Exists(path) 
            ? path 
            : null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}