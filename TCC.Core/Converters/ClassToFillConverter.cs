using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class ClassToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Class)value;
            switch (c)
            {
                case Class.Lancer:
                    return App.Current.FindResource("TankRoleColor");
                case Class.Fighter:
                    return App.Current.FindResource("TankRoleColor");
                case Class.Priest:
                    return App.Current.FindResource("HealerRoleColor");
                case Class.Elementalist:
                    return App.Current.FindResource("HealerRoleColor");
                default:
                    return App.Current.FindResource("DpsRoleColor");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}