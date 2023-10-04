using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.UI.Converters;

public class GuardianPointsStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var v =  System.Convert.ToUInt32(value);
        return v switch
        {
            100000 => "Max",
            >= 1000 => v / 1000 + "k",
            _ => v
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}