using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class RollToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int?)value)
            {
                case -1:
                    return "x";
                case 0:
                    return "-";
                default:
                    return ((int?)value).ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
