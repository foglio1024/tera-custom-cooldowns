using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TCC.Utilities;

namespace TCC.UI.Converters
{
    public class ChatChannelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ChatChannel ch)) ch = ChatChannel.Say;

            return TccUtils.ChatChannelToBrush(ch);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
