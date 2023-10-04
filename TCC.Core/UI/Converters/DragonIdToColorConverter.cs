using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.UI.Converters;

public class DragonIdToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //TODO:TRIGGERS?
        var id = (uint?)value;
        return id switch
        {
            1100 => R.Brushes.IgnidraxBrush,
            1101 => R.Brushes.TerradraxBrush,
            1102 => R.Brushes.UmbradraxBrush,
            1103 => R.Brushes.AquadraxBrush,
            _ => Brushes.Transparent
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}