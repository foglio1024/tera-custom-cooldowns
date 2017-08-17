using System;
using System.Globalization;
using System.Windows.Data;
namespace TCC.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double minOpacty = 0;
            double maxOpacity = 1;
            if (parameter != null)
            {
                var pars = ((string) parameter).Split('_');
                minOpacty = Double.Parse(pars[0], CultureInfo.InvariantCulture);
                maxOpacity = double.Parse(pars[1], CultureInfo.InvariantCulture);
            }
            if ((bool)value)
            {
                return maxOpacity;
            }
            else
            {
                return minOpacty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
