using System;
using System.Globalization;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class ClassToStaminaLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Class?) value switch
            {
                Class.Warrior => "RE",
                Class.Lancer => "RE",
                Class.Gunner => "WP",
                Class.Brawler => "RG",
                Class.Valkyrie => "RG",
                Class.Ninja => "CH",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

