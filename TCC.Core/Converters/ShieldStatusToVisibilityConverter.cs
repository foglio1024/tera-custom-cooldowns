using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TCC.Converters
{
    public class ShieldStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ShieldStatus)value)
            {
                case ShieldStatus.Off:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }

        }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}