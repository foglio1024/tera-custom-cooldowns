using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data.Map;

namespace TCC.UI.Converters;

public class LocationToStretchConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Location loc) return Stretch.Uniform;

        return Game.DB!.MapDatabase.IsDungeon(loc) 
            ? Stretch.Uniform 
            : Stretch.UniformToFill;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}