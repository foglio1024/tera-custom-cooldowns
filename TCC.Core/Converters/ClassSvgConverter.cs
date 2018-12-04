using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class ClassSvgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Class c = Class.Common;
            switch (value)
            {
                case Class cl:
                    c = cl;
                    break;
                case string s:
                    c = (Class)Enum.Parse(typeof(Class), s);
                    break;
            }

            return R.Helpers.SvgClass(c); //App.Current.FindResource("SvgClass" + c.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}