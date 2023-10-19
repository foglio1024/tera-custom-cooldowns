using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.UI.Converters;

public class AggroTypeToFillConverter : IValueConverter
{
    public Brush? Main { get; set; }
    public Brush? Secondary { get; set; }
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is AggroCircle ac ? ac : AggroCircle.None) switch
        {
            AggroCircle.Main => Main,
            AggroCircle.Secondary => Secondary,
            _ => Brushes.Transparent
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}