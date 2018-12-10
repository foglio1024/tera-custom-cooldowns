using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescr(Enum e)
        {
            var fieldInfo = e.GetType().GetField(e.ToString());

            var attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0) return e.ToString();

            var attrib = attribArray[0] as DescriptionAttribute;
            return attrib?.Description;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum e)
            {
                return GetEnumDescr(e);
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
