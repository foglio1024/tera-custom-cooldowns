using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.Converters
{
    public class AggroTypeToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            var x = (AggroCircle)value;

            switch (x)
            {
                case AggroCircle.Main:
                    return new SolidColorBrush(Colors.Orange);
                case AggroCircle.Secondary:
                    return new SolidColorBrush(Color.FromRgb(0x70, 0x40, 0xff));
                case AggroCircle.None:
                    return new SolidColorBrush(Colors.Transparent);
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}