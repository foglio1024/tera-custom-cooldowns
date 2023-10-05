using System;
using System.Globalization;
using System.Windows.Data;
using TCC.R;

namespace TCC.UI.Converters;

public class HPbarColorConverter2 : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool?) value ?? false
            ? Colors.HpDebuffColorLight //Application.Current.FindResource("HpDebuffColorLight")
            : Colors.HpColorLight; //Application.Current.FindResource("HpColorLight");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}