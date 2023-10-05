using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TeraDataLite;
using Brushes = TCC.R.Brushes;
using Colors = TCC.R.Colors;

namespace TCC.UI.Converters;

public class ClassToReColorConverter : MarkupExtension, IValueConverter
{
    public bool Light { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        var c = (Class?)value ?? Class.Common;
        var color = targetType == typeof(Color);
            
        return (c, color, Light) switch
        {
            (Class.Gunner,   true,  false) => Colors.WillpowerColorLight,
            (Class.Gunner,   true,  true)  => Colors.WillpowerColor,
            (Class.Gunner,   false, false) => Brushes.WillpowerBrush,
            (Class.Gunner,   false, true)  => Brushes.WillpowerBrushLight,
            (Class.Brawler,  true,  false) => Colors.RageColorLight,
            (Class.Brawler,  true,  true)  => Colors.RageColor,
            (Class.Brawler,  false, false) => Brushes.RageBrush,
            (Class.Brawler,  false, true)  => Brushes.RageBrushLight,
            (Class.Ninja,    true,  false) => Colors.ArcaneColorLight,
            (Class.Ninja,    true,  true)  => Colors.ArcaneColor,
            (Class.Ninja,    false, false) => Brushes.ArcaneBrush,
            (Class.Ninja,    false, true)  => Brushes.ArcaneBrushLight,
            (Class.Valkyrie, true,  _)     => System.Windows.Media.Colors.White,
            (Class.Valkyrie, false, _)     => System.Windows.Media.Brushes.White,
            _                              => null
        };
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