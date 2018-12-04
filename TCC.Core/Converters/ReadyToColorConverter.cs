using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class ReadyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ReadyStatus?)value ?? ReadyStatus.None)
            {
                case ReadyStatus.NotReady:
                    return R.Brushes.HpBrush; //Application.Current.FindResource("HpBrush");
                case ReadyStatus.Ready:
                    return R.Brushes.LightGreenBrush; //Application.Current.FindResource("LightGreenBrush");
                case ReadyStatus.Undefined:
                    return R.Brushes.GoldBrush;// Application.Current.FindResource("GoldBrush");
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
