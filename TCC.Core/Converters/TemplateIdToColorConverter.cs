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
    class TemplateIdToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (uint)value;
            if (id == 1100) return App.Current.FindResource("IgnidraxColor");
            else if (id == 1101) return App.Current.FindResource("TerradraxColor");
            else if (id == 1102) return App.Current.FindResource("UmbradraxColor");
            else if (id == 1103) return App.Current.FindResource("AquadraxColor");
            else return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
