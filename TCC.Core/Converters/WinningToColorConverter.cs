using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class WinningToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO: to triggers btw
            return (bool?) value ?? false
                ? R.Brushes.GoldBrush// new SolidColorBrush(Color.FromRgb(0xff, 0xcc, 0x00)) //TODO: to resource
                : R.Brushes.AquadraxBrush; //System.Windows.Application.Current.FindResource("AquadraxBrush"); //TODO: check color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
