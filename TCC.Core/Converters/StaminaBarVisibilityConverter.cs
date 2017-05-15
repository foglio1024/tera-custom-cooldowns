using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace TCC.Converters
{
    public class StaminaBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Class)value)
            {
                case Class.Warrior:
                    return GridLength.Auto;
                case Class.Lancer:
                    return GridLength.Auto;
                case Class.Engineer:
                    return GridLength.Auto;
                case Class.Fighter:
                    return GridLength.Auto;
                case Class.Assassin:
                    return GridLength.Auto;
                case Class.Glaiver:
                    return GridLength.Auto;
                default:
                    return new GridLength(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
