using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TCC.UI.Converters;

public class DungeonImageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var iconName = "unknown";
        if (value is string name)
        {
            iconName = name != "" ? name : iconName;
        }
        return Path.Combine(App.ResourcesPath, "images/dungeons/rp", iconName + ".jpg");

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}