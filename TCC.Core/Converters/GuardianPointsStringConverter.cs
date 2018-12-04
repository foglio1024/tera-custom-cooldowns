using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class GuardianPointsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v =  System.Convert.ToUInt32(value);
            if (v == 100000) return "Max";
            else if (v >= 1000) return v / 1000 + "k";
            else return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
