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
                    return Application.Current.FindResource("HpColor");
                case ReadyStatus.Ready:
                    return Application.Current.FindResource("LightGreenColor");
                case ReadyStatus.Undefined:
                    return Application.Current.FindResource("GoldColor");
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
