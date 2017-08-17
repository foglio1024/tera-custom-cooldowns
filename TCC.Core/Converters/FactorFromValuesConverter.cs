using System;
using System.Globalization;
using System.Windows.Data;
namespace TCC.Converters
{
    public class FactorFromValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var v = System.Convert.ToSingle(values[0]);
            var m = System.Convert.ToInt32(values[1]);
            if (m == 0)
            {
                return 1;
            }
            else
            {
                return (double)v/(double)m;
            }
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
