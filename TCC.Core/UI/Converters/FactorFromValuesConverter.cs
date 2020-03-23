using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class FactorFromValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var v = System.Convert.ToSingle(values[0]);
            var m = System.Convert.ToInt32(values[1]);
            return m == 0 ? 1 : v / (double) m;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
