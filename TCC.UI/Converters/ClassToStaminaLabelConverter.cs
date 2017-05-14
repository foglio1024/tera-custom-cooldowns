using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class ClassToStaminaLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Class)value)
            {
                case Class.Warrior:
                    return "RE";
                case Class.Lancer:
                    return "RE";
                case Class.Engineer:
                    return "WP";
                case Class.Fighter:
                    return "RG";
                case Class.Assassin:
                    return "CH";
                case Class.Glaiver:
                    return "RG";
                default:
                    return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

