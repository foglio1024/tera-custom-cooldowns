using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class DurationLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var seconds = (uint)value / 1000;
            var minutes = seconds / 60;
            var hours = minutes / 60;
            var days = hours / 24;

            if (minutes < 3) return seconds.ToString();
            if (hours < 3) return minutes + "m";
            if (days < 1) return hours + "h";
            return days + "d";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
