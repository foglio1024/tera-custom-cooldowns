using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.UI.Converters;

public class ClassToNameConverter : MarkupExtension, IValueConverter
{
    public bool ToLower { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        value ??= Class.Common;
        var ret = TccUtils.ClassEnumToString((Class)value);
        return ToLower ? ret.ToLower() : ret;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}