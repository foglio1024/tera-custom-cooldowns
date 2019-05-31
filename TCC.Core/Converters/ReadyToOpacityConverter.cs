using System;
using System.Globalization;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.Converters
{
    public class ReadyToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO: use triggers and visibility
            var v = (ReadyStatus?)value ?? ReadyStatus.None;
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
