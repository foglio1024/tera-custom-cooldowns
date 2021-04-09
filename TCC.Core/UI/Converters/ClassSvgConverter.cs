using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class ClassSvgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = value switch
            {
                Class cl => cl,
                string s => Enum.Parse<Class>(s),
                _ => Class.Common
            };

            return TccUtils.SvgClass(c); //App.Current.FindResource("SvgClass" + c.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}