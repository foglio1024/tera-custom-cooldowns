using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TeraDataLite;
using Brushes = TCC.R.Brushes;

namespace TCC.UI.Converters;

public class ClassToReColorConverter : MarkupExtension, IValueConverter
{
    public bool Light { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        var c = (Class?)value ?? Class.Common;
            
        var brush = (c, Light) switch
        {
            (Class.Gunner,   false) => Brushes.WillpowerBrush,
            (Class.Gunner,   true)  => Brushes.WillpowerBrushLight,
            (Class.Brawler,  false) => Brushes.RageBrush,
            (Class.Brawler,  true)  => Brushes.RageBrushLight,
            (Class.Ninja,    false) => Brushes.ArcaneBrush,
            (Class.Ninja,    true)  => Brushes.ArcaneBrushLight,
            (Class.Valkyrie, _)     => System.Windows.Media.Brushes.White,
            _                              => null
        };
        if(targetType == typeof(Color)) return brush?.Color;
        else return brush;
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