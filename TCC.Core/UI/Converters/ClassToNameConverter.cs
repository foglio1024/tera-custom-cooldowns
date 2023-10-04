using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.UI.Converters;

public class ClassToNameConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) value = Class.Common;
        var ret = TccUtils.ClassEnumToString((Class) value);
        var toLower = System.Convert.ToBoolean(parameter);
        return toLower ? ret.ToLower() : ret;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}