using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TCC.Utilities;

namespace TCC.UI.Converters
{
    public class ChatChannelToName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = ChatChannel.Say;
            if (value is ChatChannel cc) ch = cc;
            return TccUtils.ChatChannelToName(ch);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}