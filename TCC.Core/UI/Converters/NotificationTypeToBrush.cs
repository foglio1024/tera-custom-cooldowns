using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Utils;

namespace TCC.UI.Converters
{
    public class NotificationTypeToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is NotificationType nt)) 
                return R.Brushes.TccNormalGradientBrush;
            
            return nt switch
            {
                NotificationType.Success => R.Brushes.TccGreenGradientBrush,
                NotificationType.Warning => R.Brushes.TccYellowGradientBrush,
                NotificationType.Error => R.Brushes.TccRedGradientBrush,
                NotificationType.Info => R.Brushes.TccNormalGradientBrush,
                _ => R.Brushes.TccWhiteGradientBrush
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
