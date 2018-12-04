using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class ValueToFactorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

            var val = System.Convert.ToDouble(value);
            var max = System.Convert.ToDouble(parameter);

            if (max != 0) return val / max;
            else return 0;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
