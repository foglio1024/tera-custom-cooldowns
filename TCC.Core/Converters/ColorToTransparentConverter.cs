using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    public class ColorToTransparentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var col = Color.FromArgb(0,0,0,0);
            switch (value)
            {
                case Color c:
                    col = Color.FromArgb(0, c.R, c.G, c.B);
                    break;
                case SolidColorBrush br:
                    col = Color.FromArgb(0, br.Color.R, br.Color.G, br.Color.B);
                    break;
            }

            if(targetType == typeof(SolidColorBrush)) return new SolidColorBrush(col);
            return col;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
