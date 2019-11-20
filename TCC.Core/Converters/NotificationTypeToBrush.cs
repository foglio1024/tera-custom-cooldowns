using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TCC.Utils;

namespace TCC.Converters
{
    public class NotificationTypeToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotificationType nt)
            {
                switch (nt)
                {
                    case NotificationType.Success: return R.Brushes.TccGreenGradientBrush;
                    case NotificationType.Warning: return R.Brushes.TccYellowGradientBrush;
                    case NotificationType.Error: return R.Brushes.TccRedGradientBrush;
                }
            }

            return R.Brushes.TccNormalGradientBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
