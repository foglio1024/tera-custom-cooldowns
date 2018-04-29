using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    internal class ShieldStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ShieldStatus)value)
            {
                case ShieldStatus.On:
                    return App.Current.FindResource("MpColor");
                case ShieldStatus.Broken:
                    return App.Current.FindResource("GreenColor");
                case ShieldStatus.Failed:
                    return Brushes.Red;
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
