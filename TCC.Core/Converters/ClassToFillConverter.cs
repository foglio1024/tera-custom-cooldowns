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
                    return App.Current.FindResource("Colors.GroupWindow.Tank");
                case Class.Fighter:
                    return App.Current.FindResource("Colors.GroupWindow.Tank");
                case Class.Priest:
                    return App.Current.FindResource("Colors.GroupWindow.Healer");
                case Class.Elementalist:
                    return App.Current.FindResource("Colors.GroupWindow.Healer");
                default:
                    return App.Current.FindResource("Colors.GroupWindow.Dps");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}