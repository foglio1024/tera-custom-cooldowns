using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class AbnormalityTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (AbnormalityType)value;
            switch (val)
            {
                case AbnormalityType.Stun:
                    return new SolidColorBrush((Color)System.Windows.Application.Current.FindResource("AbnormalityBuffColor"));
                case AbnormalityType.DOT:
                    return new SolidColorBrush((Color)System.Windows.Application.Current.FindResource("AbnormalityDotColor"));
                case AbnormalityType.Debuff:
                    return new SolidColorBrush((Color)System.Windows.Application.Current.FindResource("AbnormalityDebuffColor"));
                case AbnormalityType.Buff:
                    return new SolidColorBrush((Color)System.Windows.Application.Current.FindResource("AbnormalityBuffColor"));
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}