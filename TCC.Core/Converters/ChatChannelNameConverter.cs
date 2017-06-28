using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Converters
{
    public class ChatChannelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = (ChatChannel)value;
            switch (ch)
            {
                case ChatChannel.PartyNotice:
                    return "Notice";
                case ChatChannel.RaidNotice:
                    return "Notice";
                case ChatChannel.GuildAdvertising:
                    return "G. Ad";
                case ChatChannel.Megaphone:
                    return "Shout";
                case ChatChannel.Private1:
                    return ChatWindowViewModel.Instance.PrivateChannels[0].Name;
                case ChatChannel.Private2:
                    return ChatWindowViewModel.Instance.PrivateChannels[1].Name;
                case ChatChannel.Private3:
                    return ChatWindowViewModel.Instance.PrivateChannels[2].Name;
                case ChatChannel.Private4:
                    return ChatWindowViewModel.Instance.PrivateChannels[3].Name;
                case ChatChannel.Private5:
                    return ChatWindowViewModel.Instance.PrivateChannels[4].Name;
                case ChatChannel.Private6:
                    return ChatWindowViewModel.Instance.PrivateChannels[5].Name;
                case ChatChannel.Private7:
                    return ChatWindowViewModel.Instance.PrivateChannels[6].Name;
                case ChatChannel.Private8:
                    return ChatWindowViewModel.Instance.PrivateChannels[7].Name;
                case ChatChannel.Say:
                    return ch.ToString();
                case ChatChannel.Party:
                    return ch.ToString();
                case ChatChannel.Guild:
                    return ch.ToString();
                case ChatChannel.Area:
                    return ch.ToString();
                case ChatChannel.Trade:
                    return ch.ToString();
                case ChatChannel.Greet:
                    return ch.ToString();
                case ChatChannel.Emote:
                    return ch.ToString();
                case ChatChannel.Global:
                    return ch.ToString();
                case ChatChannel.Raid:
                    return ch.ToString();
                case ChatChannel.System:
                    return ch.ToString();
                case ChatChannel.Notify:
                    return "Info";
                case ChatChannel.Event:
                    return ch.ToString();
                case ChatChannel.Error:
                    return "Alert";
                case ChatChannel.Group:
                    return ch.ToString();
                case ChatChannel.GuildNotice:
                    return "Guild";
                case ChatChannel.Deathmatch:
                    return ch.ToString();
                case ChatChannel.ContractAlert:
                    return ch.ToString();
                case ChatChannel.GroupAlerts:
                    return ch.ToString();
                case ChatChannel.Loot:
                    return ch.ToString();
                case ChatChannel.Exp:
                    return ch.ToString();
                case ChatChannel.Money:
                    return ch.ToString();
                case ChatChannel.SentWhisper:
                    return ch.ToString();
                case ChatChannel.ReceivedWhisper:
                    return ch.ToString();
                case ChatChannel.TradeRedirect:
                    return "Global";
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