using System;
using System.Globalization;
using System.Windows.Data;
namespace TCC.Converters
{
    public class ReadyToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadyStatus v = (ReadyStatus)value;
            switch (v)
            {
                case ReadyStatus.NotReady:
                    return .9;
                case ReadyStatus.Ready:
                    return .9;
                case ReadyStatus.Undefined:
                    return .9;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
