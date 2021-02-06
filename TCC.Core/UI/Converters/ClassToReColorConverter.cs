using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class ClassToReColorConverter : MarkupExtension, IValueConverter
    {
        public bool Light { get; set; }

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {

            var c = (Class?)value ?? Class.Common;
            var color = targetType == typeof(Color);
            
            return (c, color, Light) switch
            {
                (Class.Gunner,   true,  false) => R.Colors.WillpowerColorLight,
                (Class.Gunner,   true,  true)  => R.Colors.WillpowerColor,
                (Class.Gunner,   false, false) => R.Brushes.WillpowerBrush,
                (Class.Gunner,   false, true)  => R.Brushes.WillpowerBrushLight,
                (Class.Brawler,  true,  false) => R.Colors.RageColorLight,
                (Class.Brawler,  true,  true)  => R.Colors.RageColor,
                (Class.Brawler,  false, false) => R.Brushes.RageBrush,
                (Class.Brawler,  false, true)  => R.Brushes.RageBrushLight,
                (Class.Ninja,    true,  false) => R.Colors.ArcaneColorLight,
                (Class.Ninja,    true,  true)  => R.Colors.ArcaneColor,
                (Class.Ninja,    false, false) => R.Brushes.ArcaneBrush,
                (Class.Ninja,    false, true)  => R.Brushes.ArcaneBrushLight,
                (Class.Valkyrie, true,  _)     => Colors.White,
                (Class.Valkyrie, false, _)     => Brushes.White,
                _                              => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
