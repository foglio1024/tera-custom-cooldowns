using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

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
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Say");
                case ChatChannel.Party:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Party");
                case ChatChannel.Guild:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Guild");
                case ChatChannel.Area:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Area");
                case ChatChannel.Trade:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Trade");
                case ChatChannel.Greet:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Greet");
                case ChatChannel.PartyNotice:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.PartyNotice");
                case ChatChannel.RaidNotice:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.RaidNotice");
                case ChatChannel.Emote:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Emote");
                case ChatChannel.Global:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Global");
                case ChatChannel.Raid:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Raid");
                case ChatChannel.Megaphone:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Megaphone");
                case ChatChannel.GuildAdvertising:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.GuildAd");
                case ChatChannel.Private1:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private2:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private3:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private4:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private5:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private6:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private7:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Private");
                case ChatChannel.Private8:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Proxy");
                case ChatChannel.SentWhisper:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Whisper");
                case ChatChannel.ReceivedWhisper:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Whisper");
                case ChatChannel.System:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Generic");
                case ChatChannel.Notify:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Notify");
                case ChatChannel.Event:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Event");
                case ChatChannel.Error:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Error");
                case ChatChannel.Group:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Group");
                case ChatChannel.GuildNotice:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Guild");
                case ChatChannel.Deathmatch:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Deathmatch");
                case ChatChannel.ContractAlert:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.ContractAlert");
                case ChatChannel.GroupAlerts:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.GroupAlert");
                case ChatChannel.Loot:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Loot");
                case ChatChannel.Exp:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Exp");
                case ChatChannel.Money:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Money");
                case ChatChannel.TradeRedirect:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Trade");
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
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.RaidNotice");
                case ChatChannel.TCC:
                    return (SolidColorBrush)App.Current.FindResource("Colors.App.Main");
                case ChatChannel.Bargain:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Bargain");
                case ChatChannel.Apply:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.Megaphone");
                case ChatChannel.Death:
                    return (SolidColorBrush)App.Current.FindResource("Colors.App.HP");
                case ChatChannel.Ress:
                    return (SolidColorBrush)App.Current.FindResource("Colors.App.LightGreen");
                case ChatChannel.Quest:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Quest");
                case ChatChannel.Friend:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Friend");
                case ChatChannel.Twitch:
                    return (SolidColorBrush)App.Current.FindResource("Colors.App.Twitch");
                case ChatChannel.WorldBoss:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.WorldBoss");
                case ChatChannel.Laurel:
                    return Brushes.OrangeRed;
                default:
                    return (SolidColorBrush)App.Current.FindResource("Colors.Chat.System.Generic");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
