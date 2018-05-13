using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class ClassToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) value = Class.Common;
            var ret = Utils.ClassEnumToString((Class) value);
            var toLower = System.Convert.ToBoolean(parameter);
            if (toLower) return ret.ToLower();
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}