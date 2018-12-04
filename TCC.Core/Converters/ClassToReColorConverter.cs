using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class ClassToReColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var col = Colors.Transparent;
            var light = parameter != null && parameter.ToString().Contains("light");
            //var parLight = parameter != null && parameter.ToString().Contains("light") ? "Light" : "";
            if (value != null)
                switch ((Class)value)
                {
                    case Class.Gunner:
                        col = light ? R.Colors.WillpowerColorLight : R.Colors.WillpowerColor; // (Color) Application.Current.FindResource("WillpowerColor"+parLight);
                        break;
                    case Class.Brawler:
                        col = light ? R.Colors.RageColorLight : R.Colors.RageColor; //(Color)Application.Current.FindResource("RageColor" + parLight);
                        break;
                    case Class.Ninja:
                        col = light ? R.Colors.ArcaneColorLight : R.Colors.ArcaneColor; // (Color) Application.Current.FindResource("ArcaneColor"+parLight);
                        break;
                    case Class.Valkyrie:
                        col = Colors.White;
                        break;
                }
            return parameter != null && parameter.ToString().Contains("color") ? (object)col : new SolidColorBrush(col);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
