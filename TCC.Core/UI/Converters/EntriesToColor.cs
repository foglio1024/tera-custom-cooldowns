using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.UI.Converters;

public class EntriesToColor : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var retCol = Colors.Transparent;
        if (values[0] is not int entries || values[1] is not int max) { return targetType == typeof(Brush) ? new SolidColorBrush(retCol) : retCol; }

        retCol = R.Colors.Tier3DungeonColor;
        if (entries == max) retCol = R.Colors.GreenColor;
        else if (entries == 0) retCol = R.Colors.AssaultStanceColor;

        return targetType == typeof(Brush) ? new SolidColorBrush(retCol) : retCol;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}