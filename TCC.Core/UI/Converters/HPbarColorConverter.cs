using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.UI.Converters;

public class HPbarColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool?) value ?? false
            ? R.Colors.HpDebuffColor // Application.Current.FindResource("HpDebuffColor")
            : R.Colors.HpColor; // Application.Current.FindResource("HpColor");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}