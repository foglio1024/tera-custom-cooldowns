using System;
using System.Globalization;
using System.Windows.Data;
using TCC.R;

namespace TCC.UI.Converters;

public class DragonIdToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //TODO:TRIGGERS?
        var id = (uint?)value;
        return id switch
        {
            1100 => Brushes.IgnidraxBrush,
            1101 => Brushes.TerradraxBrush,
            1102 => Brushes.UmbradraxBrush,
            1103 => Brushes.AquadraxBrush,
            _ => System.Windows.Media.Brushes.Transparent
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}