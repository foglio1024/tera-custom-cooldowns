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
                    return Visibility.Visible;
                case Class.Lancer:
                    return Visibility.Visible;
                //case Class.Engineer:
                //    return Visibility.Visible;
                //case Class.Fighter:
                //    return Visibility.Visible;
                //case Class.Assassin:
                //    return Visibility.Visible;
                //case Class.Glaiver:
                //    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
