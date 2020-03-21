using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.UI.Converters
{
    public class ChatChannelToName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = ChatChannel.Say;
            if (value is ChatChannel cc) ch = cc;
            return ch switch
            {
                ChatChannel.PartyNotice => "Notice",
                ChatChannel.RaidNotice => "Notice",
                ChatChannel.GuildAdvertising => "G. Ad",
                ChatChannel.Megaphone => "Megaphone",
                ChatChannel.Private1 => (ChatManager.Instance.PrivateChannels[0].Name ?? ch.ToString()),
                ChatChannel.Private2 => (ChatManager.Instance.PrivateChannels[1].Name ?? ch.ToString()),
                ChatChannel.Private3 => (ChatManager.Instance.PrivateChannels[2].Name ?? ch.ToString()),
                ChatChannel.Private4 => (ChatManager.Instance.PrivateChannels[3].Name ?? ch.ToString()),
                ChatChannel.Private5 => (ChatManager.Instance.PrivateChannels[4].Name ?? ch.ToString()),
                ChatChannel.Private6 => (ChatManager.Instance.PrivateChannels[5].Name ?? ch.ToString()),
                ChatChannel.Private7 => (ChatManager.Instance.PrivateChannels[6].Name ?? ch.ToString()),
                ChatChannel.Private8 => (ChatManager.Instance.PrivateChannels[7].Name ?? ch.ToString()),
                ChatChannel.Notify => "Info",
                ChatChannel.Error => "Alert",
                ChatChannel.GuildNotice => "Guild",
                ChatChannel.GroupAlerts => "Group",
                ChatChannel.TradeRedirect => "Global",
                ChatChannel.Enchant => "Gear",
                ChatChannel.RaidLeader => "Leader",
                ChatChannel.Bargain => "Offer",
                ChatChannel.WorldBoss => "W.B.",
                ChatChannel.SystemDefault => "System",
                ChatChannel.Damage => "Dmg",
                ChatChannel.Guardian => "G.L.",
                ChatChannel.ReceivedWhisper => "Whisper",
                _ => ch.ToString()
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}