using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class ChatChannelToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = (ChatChannel)value;
            switch (ch)
            {
                case ChatChannel.Say:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Say");
                case ChatChannel.Party:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Party");
                case ChatChannel.Guild:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Guild");
                case ChatChannel.Area:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Area");
                case ChatChannel.Trade:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Trade");
                case ChatChannel.Greet:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Greet");
                case ChatChannel.PartyNotice:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.PartyNotice");
                case ChatChannel.RaidNotice:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.RaidNotice");
                case ChatChannel.Emote:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Emote");
                case ChatChannel.Global:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Global");
                case ChatChannel.Raid:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Raid");
                case ChatChannel.Megaphone:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Megaphone");
                case ChatChannel.GuildAdvertising:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.GuildAd");
                case ChatChannel.Private1:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private2:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private3:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private4:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private5:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private6:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private7:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Proxy");
                case ChatChannel.Private8:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Proxy");
                case ChatChannel.SentWhisper:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Whisper");
                case ChatChannel.ReceivedWhisper:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Whisper");
                case ChatChannel.System:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Generic");
                case ChatChannel.Notify:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Notify");
                case ChatChannel.Event:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Event");
                case ChatChannel.Error:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Error");
                case ChatChannel.Group:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Group");
                case ChatChannel.GuildNotice:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Guild");
                case ChatChannel.Deathmatch:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Deathmatch");
                case ChatChannel.ContractAlert:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.ContractAlert");
                case ChatChannel.GroupAlerts:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.GroupAlert");
                case ChatChannel.Loot:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Loot");
                case ChatChannel.Exp:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Exp");
                case ChatChannel.Money:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Money");
                case ChatChannel.TradeRedirect:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Trade");
                case ChatChannel.Enchant12:
                    return Brushes.Orange;
                case ChatChannel.Enchant15:
                    return Brushes.OrangeRed;
                case ChatChannel.Enchant7:
                    return Brushes.Orange;
                case ChatChannel.Enchant8:
                    return Brushes.Orange;
                case ChatChannel.Enchant9:
                    return Brushes.OrangeRed;
                case ChatChannel.RaidLeader:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.RaidNotice");
                case ChatChannel.TCC:
                    return (SolidColorBrush)Application.Current.FindResource("MainColor");
                case ChatChannel.Bargain:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Bargain");
                case ChatChannel.Apply:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.Megaphone");
                case ChatChannel.Death:
                case ChatChannel.Damage:
                    return (SolidColorBrush)Application.Current.FindResource("HpColor");
                case ChatChannel.Ress:
                    return (SolidColorBrush)Application.Current.FindResource("LightGreenColor");
                case ChatChannel.Quest:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Quest");
                case ChatChannel.Friend:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Friend");
                case ChatChannel.Twitch:
                    return (SolidColorBrush)Application.Current.FindResource("TwitchColor");
                case ChatChannel.WorldBoss:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.WorldBoss");
                case ChatChannel.Laurel:
                    return Brushes.OrangeRed;
                default:
                    return (SolidColorBrush)Application.Current.FindResource("Colors.Chat.System.Generic");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
