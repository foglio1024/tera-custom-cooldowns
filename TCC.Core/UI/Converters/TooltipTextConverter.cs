using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TCC.UI.Controls.Abnormalities;

namespace TCC.UI.Converters;

public class TooltipTextConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string tt) return new TooltipParser(tt).Parse();
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}