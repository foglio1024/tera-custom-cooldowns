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
            if (value != null)
                switch ((Class)value)
                {
                    case Class.Gunner:
                        col = Colors.Aqua;
                        break;
                    case Class.Brawler:
                        col = Colors.Orange;
                        break;
                    case Class.Ninja:
                        // ReSharper disable once PossibleNullReferenceException
                        col = (Color) Application.Current.FindResource("NinjaColor");
                        break;
                    case Class.Valkyrie:
                        col = Colors.White;
                        break;
                }
            return parameter != null && parameter.ToString() == "color" ? (object)col : new SolidColorBrush(col);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
