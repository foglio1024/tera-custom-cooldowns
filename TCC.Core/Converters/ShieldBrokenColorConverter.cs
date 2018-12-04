using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class ShieldStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ShieldStatus?)value) //TODO: triggers
            {
                case ShieldStatus.On:
                    return R.Brushes.MpBrush; // Application.Current.FindResource("MpBrush");
                case ShieldStatus.Broken:
                    return R.Brushes.GreenBrush; //Application.Current.FindResource("GreenBrush");
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
