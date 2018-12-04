using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class ChatChannelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = (ChatChannel?)value ?? ChatChannel.Say;
            switch (ch)
            {
                case ChatChannel.Say:
                    return R.Brushes.ChatSayBrush;
                case ChatChannel.Party:
                    return R.Brushes.ChatPartyBrush;
                case ChatChannel.Guild:
                    return R.Brushes.ChatGuildBrush;
                case ChatChannel.Area:
                    return R.Brushes.ChatAreaBrush;
                case ChatChannel.Trade:
                    return R.Brushes.ChatTradeBrush;
                case ChatChannel.Greet:
                    return R.Brushes.ChatGreetBrush;
                case ChatChannel.PartyNotice:
                    return R.Brushes.ChatPartyNoticeBrush;
                case ChatChannel.RaidNotice:
                    return R.Brushes.ChatRaidNoticeBrush;
                case ChatChannel.Emote:
                    return R.Brushes.ChatEmoteBrush;
                case ChatChannel.Global:
                    return R.Brushes.ChatGlobalBrush;
                case ChatChannel.Raid:
                    return R.Brushes.ChatRaidBrush;
                case ChatChannel.Megaphone:
                    return R.Brushes.ChatMegaphoneBrush;
                case ChatChannel.GuildAdvertising:
                    return R.Brushes.ChatGuildAdBrush;
                case ChatChannel.Private1:
                case ChatChannel.Private2:
                case ChatChannel.Private3:
                case ChatChannel.Private4:
                case ChatChannel.Private5:
                case ChatChannel.Private6:
                    return R.Brushes.ChatPrivateBrush;
                case ChatChannel.Private7:
                case ChatChannel.Private8:
                    return R.Brushes.ChatProxyBrush;
                case ChatChannel.SentWhisper:
                case ChatChannel.ReceivedWhisper:
                    return R.Brushes.ChatWhisperBrush;
                case ChatChannel.System:
                    return R.Brushes.ChatSystemGenericBrush;
                case ChatChannel.Notify:
                    return R.Brushes.ChatSystemNotifyBrush;
                case ChatChannel.Event:
                    return R.Brushes.ChatSystemEventBrush;
                case ChatChannel.Error:
                    return R.Brushes.ChatSystemErrorBrush;
                case ChatChannel.Group:
                    return R.Brushes.ChatSystemGroupBrush;
                case ChatChannel.GuildNotice:
                    return R.Brushes.ChatGuildBrush;
                case ChatChannel.Deathmatch:
                    return R.Brushes.ChatSystemDeathmatchBrush;
                case ChatChannel.ContractAlert:
                    return R.Brushes.ChatSystemContractAlertBrush;
                case ChatChannel.GroupAlerts:
                    return R.Brushes.ChatSystemGroupAlertBrush;
                case ChatChannel.Loot:
                    return R.Brushes.ChatSystemLootBrush;
                case ChatChannel.Exp:
                    return R.Brushes.ChatSystemExpBrush;
                case ChatChannel.Money:
                    return R.Brushes.ChatSystemMoneyBrush;
                case ChatChannel.TradeRedirect:
                    return R.Brushes.ChatTradeBrush;
                //case ChatChannel.Enchant12:
                //case ChatChannel.Enchant7:
                //case ChatChannel.Enchant8:
                case ChatChannel.Enchant:
                    return R.Brushes.EnchantLowBrush;
                //case ChatChannel.Enchant9:
                case ChatChannel.Laurel:
                //case ChatChannel.Enchant15:
                    return R.Brushes.EnchantHighBrush;
                case ChatChannel.RaidLeader:
                    return R.Brushes.ChatRaidNoticeBrush;
                case ChatChannel.TCC:
                    return R.Brushes.MainBrush;
                case ChatChannel.Bargain:
                    return R.Brushes.ChatSystemBargainBrush;
                case ChatChannel.Apply:
                    return R.Brushes.ChatMegaphoneBrush;
                case ChatChannel.Death:
                case ChatChannel.Damage:
                    return R.Brushes.HpBrush;
                case ChatChannel.Ress:
                    return R.Brushes.GreenBrush;
                case ChatChannel.Quest:
                    return R.Brushes.ChatSystemQuestBrush;
                case ChatChannel.Friend:
                    return R.Brushes.ChatSystemFriendBrush;
                case ChatChannel.Twitch:
                    return R.Brushes.TwitchBrush;
                case ChatChannel.WorldBoss:
                    return R.Brushes.ChatSystemWorldBossBrush;
                case ChatChannel.Guardian:
                    return R.Brushes.GuardianBrush;
                default:
                    return R.Brushes.ChatSystemGenericBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
