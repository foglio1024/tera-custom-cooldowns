using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.UI.Converters;

public class ClassToFillConverter : IValueConverter
{
    public SolidColorBrush? Dps { get; set; }
    public SolidColorBrush? Tank { get; set; }
    public SolidColorBrush? Healer { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Class c) c = Class.Common;

        var brush = c switch
        {
            Class.Lancer or Class.Brawler => Tank,
            Class.Priest or Class.Mystic => Healer,
            Class.Common => Brushes.White,
            _ => Dps,
        };
        if (targetType == typeof(Color)) return brush?.Color;
        else return brush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}