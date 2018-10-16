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
            var ch = (ChatChannel?)value ?? ChatChannel.Say;
            string resName;
            switch (ch)
            {
                case ChatChannel.Say:
                    resName = "ChatSayBrush"; break;
                case ChatChannel.Party:
                    resName = "ChatPartyBrush"; break;
                case ChatChannel.Guild:
                    resName = "ChatGuildBrush"; break;
                case ChatChannel.Area:
                    resName = "ChatAreaBrush"; break;
                case ChatChannel.Trade:
                    resName = "ChatTradeBrush"; break;
                case ChatChannel.Greet:
                    resName = "ChatGreetBrush"; break;
                case ChatChannel.PartyNotice:
                    resName = "ChatPartyNoticeBrush"; break;
                case ChatChannel.RaidNotice:
                    resName = "ChatRaidNoticeBrush"; break;
                case ChatChannel.Emote:
                    resName = "ChatEmoteBrush"; break;
                case ChatChannel.Global:
                    resName = "ChatGlobalBrush"; break;
                case ChatChannel.Raid:
                    resName = "ChatRaidBrush"; break;
                case ChatChannel.Megaphone:
                    resName = "ChatMegaphoneBrush"; break;
                case ChatChannel.GuildAdvertising:
                    resName = "ChatGuildAdBrush"; break;
                case ChatChannel.Private1:
                case ChatChannel.Private2:
                case ChatChannel.Private3:
                case ChatChannel.Private4:
                case ChatChannel.Private5:
                case ChatChannel.Private6:
                    resName = "ChatPrivateBrush"; break;
                case ChatChannel.Private7:
                case ChatChannel.Private8:
                    resName = "ChatProxyBrush"; break;
                case ChatChannel.SentWhisper:
                case ChatChannel.ReceivedWhisper:
                    resName = "ChatWhisperBrush"; break;
                case ChatChannel.System:
                    resName = "ChatSystemGenericBrush"; break;
                case ChatChannel.Notify:
                    resName = "ChatSystemNotifyBrush"; break;
                case ChatChannel.Event:
                    resName = "ChatSystemEventBrush"; break;
                case ChatChannel.Error:
                    resName = "ChatSystemErrorBrush"; break;
                case ChatChannel.Group:
                    resName = "ChatSystemGroupBrush"; break;
                case ChatChannel.GuildNotice:
                    resName = "ChatGuildBrush"; break;
                case ChatChannel.Deathmatch:
                    resName = "ChatSystemDeathmatchBrush"; break;
                case ChatChannel.ContractAlert:
                    resName = "ChatSystemContractAlertBrush"; break;
                case ChatChannel.GroupAlerts:
                    resName = "ChatSystemGroupAlertBrush"; break;
                case ChatChannel.Loot:
                    resName = "ChatSystemLootBrush"; break;
                case ChatChannel.Exp:
                    resName = "ChatSystemExpBrush"; break;
                case ChatChannel.Money:
                    resName = "ChatSystemMoneyBrush"; break;
                case ChatChannel.TradeRedirect:
                    resName = "ChatTradeBrush"; break;
                //case ChatChannel.Enchant12:
                //case ChatChannel.Enchant7:
                //case ChatChannel.Enchant8:
                case ChatChannel.Enchant:
                    resName = "EnchantLowBrush"; break;
                //case ChatChannel.Enchant9:
                case ChatChannel.Laurel:
                //case ChatChannel.Enchant15:
                    resName = "EnchantHighBrush"; break;
                case ChatChannel.RaidLeader:
                    resName = "ChatRaidNoticeBrush"; break;
                case ChatChannel.TCC:
                    resName = "MainBrush"; break;
                case ChatChannel.Bargain:
                    resName = "ChatSystemBargainBrush"; break;
                case ChatChannel.Apply:
                    resName = "ChatMegaphoneBrush"; break;
                case ChatChannel.Death:
                case ChatChannel.Damage:
                    resName = "HpBrush"; break;
                case ChatChannel.Ress:
                    resName = "LightGreenBrush"; break;
                case ChatChannel.Quest:
                    resName = "ChatSystemQuestBrush"; break;
                case ChatChannel.Friend:
                    resName = "ChatSystemFriendBrush"; break;
                case ChatChannel.Twitch:
                    resName = "TwitchBrush"; break;
                case ChatChannel.WorldBoss:
                    resName = "ChatSystemWorldBossBrush"; break;
                case ChatChannel.Guardian:
                    resName = "GuardianBrush"; break;
                default:
                    resName = "ChatSystemGenericBrush"; break;
            }
            return (SolidColorBrush)Application.Current.FindResource(resName);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
