using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.Converters
{
    public class ReadyToSvgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReadyStatus r)) r = ReadyStatus.None;

            return r switch
            {
                ReadyStatus.Undefined => R.Nostrum_SVG.SvgMinimize,
                ReadyStatus.NotReady => R.Nostrum_SVG.SvgClose,
                ReadyStatus.Ready => R.Nostrum_SVG.SvgConfirm,
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
