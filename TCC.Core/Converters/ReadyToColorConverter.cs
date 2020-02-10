using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.Converters
{
    public class ReadyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReadyStatus r)) r = ReadyStatus.None;
            return r switch
            {
                ReadyStatus.NotReady => R.Brushes.HpBrush,
                ReadyStatus.Ready => R.Brushes.LightGreenBrush,
                ReadyStatus.Undefined => R.Brushes.GoldBrush,
                _ => Brushes.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
