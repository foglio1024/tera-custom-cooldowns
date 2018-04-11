using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class RollToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((int)value == -1)
            {
                return "x";
            }
            else if((int)value == 0)
            {
                return "-";
            }
            else
            {
                return ((int)value).ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
