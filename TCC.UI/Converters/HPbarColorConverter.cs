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
                return new SolidColorBrush(Color.FromRgb(0xb7, 0x4b, 0xe5));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0xd0, 0, 0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
