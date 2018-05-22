using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;

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
                //case Class.Gunner:
                //    return Visibility.Visible;
                //case Class.Brawler:
                //    return Visibility.Visible;
                //case Class.Ninja:
                //    return Visibility.Visible;
                //case Class.Valkyrie:
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
