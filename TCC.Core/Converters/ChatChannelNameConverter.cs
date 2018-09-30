using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Converters
{
    public class ChatChannelToName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = (ChatChannel?) value ?? ChatChannel.Say;
            switch (ch)
            {
                case ChatChannel.PartyNotice:
                case ChatChannel.RaidNotice:
                    return "Notice";
                case ChatChannel.GuildAdvertising:
                    return "G. Ad";
                case ChatChannel.Megaphone:
                    return "Megaphone";
                case ChatChannel.Private1:
                    return ChatWindowManager.Instance.PrivateChannels[0].Name ?? ch.ToString();
                case ChatChannel.Private2:
                    return ChatWindowManager.Instance.PrivateChannels[1].Name ?? ch.ToString();
                case ChatChannel.Private3:
                    return ChatWindowManager.Instance.PrivateChannels[2].Name ?? ch.ToString();
                case ChatChannel.Private4:
                    return ChatWindowManager.Instance.PrivateChannels[3].Name ?? ch.ToString();
                case ChatChannel.Private5:
                    return ChatWindowManager.Instance.PrivateChannels[4].Name ?? ch.ToString();
                case ChatChannel.Private6:
                    return ChatWindowManager.Instance.PrivateChannels[5].Name ?? ch.ToString();
                case ChatChannel.Private7:
                    return ChatWindowManager.Instance.PrivateChannels[6].Name ?? ch.ToString();
                case ChatChannel.Private8:
                    return ChatWindowManager.Instance.PrivateChannels[7].Name ?? ch.ToString();
                case ChatChannel.Notify:
                    return "Info";
                case ChatChannel.Error:
                    return "Alert";
                case ChatChannel.GuildNotice:
                    return "Guild";
                case ChatChannel.GroupAlerts:
                    return "Group";
                case ChatChannel.TradeRedirect:
                    return "Global";
                case ChatChannel.Enchant12:
                    return "+12";
                case ChatChannel.Enchant15:
                    return "+15";
                case ChatChannel.Enchant7:
                    return "+7";
                case ChatChannel.Enchant8:
                    return "+8";
                case ChatChannel.Enchant9:
                    return "+9";
                case ChatChannel.RaidLeader:
                    return "Leader";
                case ChatChannel.Bargain:
                    return "Offer";
                case ChatChannel.WorldBoss:
                    return "WB";
                case ChatChannel.SystemDefault:
                    return "System";
                case ChatChannel.Damage:
                    return "Dmg";
                default:
                    return ch.ToString();
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}