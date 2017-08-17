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
    class ShieldStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ShieldStatus)value)
            {
                case ShieldStatus.On:
                    return App.Current.FindResource("Colors.App.MP");
                case ShieldStatus.Broken:
                    return App.Current.FindResource("Colors.App.Green");
                case ShieldStatus.Failed:
                    return Brushes.Red;
                default:
                    return Brushes.Transparent;
            }
        }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
}
}
