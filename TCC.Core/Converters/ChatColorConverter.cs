using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    public class ChatColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = (ChatChannel)value;
            switch (ch)
            {
                case ChatChannel.Say:
                    return (SolidColorBrush)App.Current.FindResource("ChatSay");
                case ChatChannel.Party:
                    return (SolidColorBrush)App.Current.FindResource("ChatParty");
                case ChatChannel.Guild:
                    return (SolidColorBrush)App.Current.FindResource("ChatGuild");
                case ChatChannel.Area:
                    return (SolidColorBrush)App.Current.FindResource("ChatArea");
                case ChatChannel.Trade:
                    return (SolidColorBrush)App.Current.FindResource("ChatTrade");
                case ChatChannel.Greet:
                    return (SolidColorBrush)App.Current.FindResource("ChatGreet");
                case ChatChannel.PartyNotice:
                    return (SolidColorBrush)App.Current.FindResource("ChatPartyNotice");
                case ChatChannel.RaidNotice:
                    return (SolidColorBrush)App.Current.FindResource("ChatRaidNotice");
                case ChatChannel.Emote:
                    return (SolidColorBrush)App.Current.FindResource("ChatEmote");
                case ChatChannel.Global:
                    return (SolidColorBrush)App.Current.FindResource("ChatGlobal");
                case ChatChannel.Raid:
                    return (SolidColorBrush)App.Current.FindResource("ChatRaid");
                case ChatChannel.Megaphone:
                    return (SolidColorBrush)App.Current.FindResource("ChatMegaphone");
                case ChatChannel.GuildAdvertising:
                    return (SolidColorBrush)App.Current.FindResource("ChatGuildAd");
                case ChatChannel.Private1:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.Private2:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.Private3:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.Private4:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.Private5:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.Private6:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.Private7:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.Private8:
                    return (SolidColorBrush)App.Current.FindResource("ChatPrivate");
                case ChatChannel.SentWhisper:
                    return (SolidColorBrush)App.Current.FindResource("ChatWhisper");
                case ChatChannel.ReceivedWhisper:
                    return (SolidColorBrush)App.Current.FindResource("ChatWhisper");
                case ChatChannel.System:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysGeneric");
                case ChatChannel.Notify:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysNotify");
                case ChatChannel.Event:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysEvent");
                case ChatChannel.Error:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysError");
                case ChatChannel.Group:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysGroup");
                case ChatChannel.GuildNotice:
                    return (SolidColorBrush)App.Current.FindResource("ChatGuild");
                case ChatChannel.Deathmatch:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysDeathmatch");
                case ChatChannel.ContractAlert:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysContractAlert");
                case ChatChannel.GroupAlerts:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysGroupAlert");
                case ChatChannel.Loot:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysLoot");
                case ChatChannel.Exp:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysExp");
                case ChatChannel.Money:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysMoney");
                case ChatChannel.TradeRedirect:
                    return (SolidColorBrush)App.Current.FindResource("ChatTrade");
                case ChatChannel.Enchant12:
                    return Brushes.Orange;
                case ChatChannel.Enchant15:
                    return Brushes.OrangeRed;
                case ChatChannel.RaidLeader:
                    return (SolidColorBrush)App.Current.FindResource("ChatRaidNotice");
                case ChatChannel.TCC:
                    return (SolidColorBrush)App.Current.FindResource("mainTccColor");
                case ChatChannel.Bargain:
                    return (SolidColorBrush)App.Current.FindResource("ChatBargain");
                case ChatChannel.Apply:
                    return (SolidColorBrush)App.Current.FindResource("ChatMegaphone");
                case ChatChannel.Death:
                    return Brushes.Red;
                case ChatChannel.Ress:
                    return (SolidColorBrush)App.Current.FindResource("greenTccColor");
                default:
                    return (SolidColorBrush)App.Current.FindResource("ChatSysGeneric");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
