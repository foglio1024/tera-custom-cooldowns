using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace TCC.Converters
{
    public class ReadyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ReadyStatus)value)
            {
                case ReadyStatus.NotReady:
                    return App.Current.FindResource("HpColor");
                case ReadyStatus.Ready:
                    return App.Current.FindResource("LightGreenColor");
                case ReadyStatus.Undefined:
                    return App.Current.FindResource("GoldColor");
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
