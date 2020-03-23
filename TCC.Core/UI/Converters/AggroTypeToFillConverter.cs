using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class AggroTypeToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AggroCircle?)value switch
            {
                AggroCircle.Main => R.Brushes.GoldBrush,
                AggroCircle.Secondary => R.Brushes.TwitchBrush,
                _ => Brushes.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}