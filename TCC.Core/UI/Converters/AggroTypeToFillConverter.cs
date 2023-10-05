using System;
using System.Globalization;
using System.Windows.Data;
using TCC.R;
using TeraDataLite;

namespace TCC.UI.Converters;

public class AggroTypeToFillConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is AggroCircle ac ? ac : AggroCircle.None) switch
        {
            AggroCircle.Main => Brushes.GoldBrush,
            AggroCircle.Secondary => Brushes.TwitchBrush,
            _ => System.Windows.Media.Brushes.Transparent
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}