using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

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
                case Class.Gunner:
                    return "WP";
                case Class.Brawler:
                    return "RG";
                case Class.Ninja:
                    return "CH";
                case Class.Valkyrie:
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

