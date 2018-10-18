using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class ClassToReColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color col = Colors.Transparent;
            var parLight = parameter != null && parameter.ToString().Contains("light") ? "Light" : "";
            if (value != null)
                switch ((Class)value)
                {
                    case Class.Gunner:
                        // ReSharper disable once PossibleNullReferenceException
                        col = (Color) Application.Current.FindResource("WillpowerColor"+parLight);
                        break;
                    case Class.Brawler:
                        // ReSharper disable once PossibleNullReferenceException
                        col = (Color)Application.Current.FindResource("RageColor" + parLight);
                        break;
                    case Class.Ninja:
                        // ReSharper disable once PossibleNullReferenceException
                        col = (Color) Application.Current.FindResource("ArcaneColor"+parLight);
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
