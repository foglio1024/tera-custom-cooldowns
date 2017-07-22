using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace TCC.Converters
{
    public class HPbarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return ((SolidColorBrush)App.Current.FindResource("Colors.App.HP.Debuff"));
            }
            else
            {
                return ((SolidColorBrush)App.Current.FindResource("Colors.App.HP"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
