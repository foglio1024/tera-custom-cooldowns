using System;
using System.Globalization;
using System.Windows.Data;
using TCC.R;
using TCC.Utils;

namespace TCC.UI.Converters;

public class NotificationTypeToBrush : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NotificationType nt) 
            return Brushes.TccNormalGradientBrush;
            
        return nt switch
        {
            NotificationType.Success => Brushes.TccGreenGradientBrush,
            NotificationType.Warning => Brushes.TccYellowGradientBrush,
            NotificationType.Error => Brushes.TccRedGradientBrush,
            NotificationType.Info => Brushes.TccNormalGradientBrush,
            _ => Brushes.TccWhiteGradientBrush
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}