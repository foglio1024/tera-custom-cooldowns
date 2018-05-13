using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class ReadyToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (ReadyStatus)value;
            var img = "resources/images/Icon_Laurels/blank.png";
            switch (v)
            {
                case ReadyStatus.NotReady:
                    img = "resources/images/ic_close_white_24dp_2x.png";
                    break;
                case ReadyStatus.Ready:
                    img = "resources/images/ic_done_white_24dp_2x.png";
                    break;
                case ReadyStatus.Undefined:
                    img = "resources/images/ic_remove_white_24dp_2x.png";
                    break;
                default:
                    break;
            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
