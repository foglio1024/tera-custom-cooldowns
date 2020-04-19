using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class ClassToReColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var col = Colors.White;
            var light = parameter != null && parameter.ToString().Contains("light");
            if (!(value is Class c))
                return parameter != null && parameter.ToString().Contains("color")
                    ? (object)col
                    : new SolidColorBrush(col);

            col = c switch
            {
                Class.Gunner =>
                (light
                    ? R.Colors.WillpowerColorLight
                    : R.Colors.WillpowerColor)
                ,
                Class.Brawler =>
                (light
                    ? R.Colors.RageColorLight
                    : R.Colors.RageColor)
                ,
                Class.Ninja =>
                (light
                    ? R.Colors.ArcaneColorLight
                    : R.Colors.ArcaneColor)
                ,
                Class.Valkyrie => Colors.White,
                _ => col
            };
            return parameter != null && parameter.ToString().Contains("color") ? (object)col : new SolidColorBrush(col);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
