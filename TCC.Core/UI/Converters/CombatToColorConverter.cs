using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.UI.Converters
{
    public class CombatToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (bool?)value ?? false;

            return c ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
