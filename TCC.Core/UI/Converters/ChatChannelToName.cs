using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Utilities;
using TCC.Utils;

namespace TCC.UI.Converters;

public class ChatChannelToName : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ChatChannel cc) cc = ChatChannel.Say;
        return TccUtils.ChatChannelToName(cc);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}