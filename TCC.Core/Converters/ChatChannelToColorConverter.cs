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
            if (!(value is ChatChannel ch)) ch = ChatChannel.Say;

            return ch switch
            {
                ChatChannel.Say => R.Brushes.ChatSayBrush,
                ChatChannel.Party => R.Brushes.ChatPartyBrush,
                ChatChannel.Guild => R.Brushes.ChatGuildBrush,
                ChatChannel.Area => R.Brushes.ChatAreaBrush,
                ChatChannel.Trade => R.Brushes.ChatTradeBrush,
                ChatChannel.Greet => R.Brushes.ChatGreetBrush,
                ChatChannel.Angler => R.Brushes.ChatGreetBrush,
                ChatChannel.PartyNotice => R.Brushes.ChatPartyNoticeBrush,
                ChatChannel.RaidNotice => R.Brushes.ChatRaidNoticeBrush,
                ChatChannel.Emote => R.Brushes.ChatEmoteBrush,
                ChatChannel.Global => R.Brushes.ChatGlobalBrush,
                ChatChannel.Raid => R.Brushes.ChatRaidBrush,
                ChatChannel.GuildAdvertising => R.Brushes.ChatGuildAdBrush,
                ChatChannel.Private1 => R.Brushes.ChatPrivateBrush,
                ChatChannel.Private2 => R.Brushes.ChatPrivateBrush,
                ChatChannel.Private3 => R.Brushes.ChatPrivateBrush,
                ChatChannel.Private4 => R.Brushes.ChatPrivateBrush,
                ChatChannel.Private5 => R.Brushes.ChatPrivateBrush,
                ChatChannel.Private6 => R.Brushes.ChatPrivateBrush,
                ChatChannel.Private7 => R.Brushes.ChatProxyBrush,
                ChatChannel.Private8 => R.Brushes.ChatProxyBrush,
                ChatChannel.SentWhisper => R.Brushes.ChatWhisperBrush,
                ChatChannel.ReceivedWhisper => R.Brushes.ChatWhisperBrush,
                ChatChannel.System => R.Brushes.ChatSystemGenericBrush,
                ChatChannel.Notify => R.Brushes.ChatSystemNotifyBrush,
                ChatChannel.Event => R.Brushes.ChatSystemEventBrush,
                ChatChannel.Error => R.Brushes.ChatSystemErrorBrush,
                ChatChannel.Group => R.Brushes.ChatSystemGroupBrush,
                ChatChannel.GuildNotice => R.Brushes.ChatGuildBrush,
                ChatChannel.Deathmatch => R.Brushes.ChatSystemDeathmatchBrush,
                ChatChannel.ContractAlert => R.Brushes.ChatSystemContractAlertBrush,
                ChatChannel.GroupAlerts => R.Brushes.ChatSystemGroupAlertBrush,
                ChatChannel.Loot => R.Brushes.ChatSystemLootBrush,
                ChatChannel.Exp => R.Brushes.ChatSystemExpBrush,
                ChatChannel.Money => R.Brushes.ChatSystemMoneyBrush,
                ChatChannel.TradeRedirect => R.Brushes.ChatTradeBrush,
                ChatChannel.Enchant => R.Brushes.EnchantLowBrush,
                ChatChannel.Laurel => R.Brushes.EnchantHighBrush,
                ChatChannel.RaidLeader => R.Brushes.ChatRaidNoticeBrush,
                ChatChannel.TCC => R.Brushes.MainBrush,
                ChatChannel.Bargain => R.Brushes.ChatSystemBargainBrush,
                ChatChannel.Apply => R.Brushes.ChatMegaphoneBrush,
                ChatChannel.LFG => R.Brushes.ChatMegaphoneBrush,
                ChatChannel.Megaphone => R.Brushes.ChatMegaphoneBrush,
                ChatChannel.Death => R.Brushes.HpBrush,
                ChatChannel.Damage => R.Brushes.HpBrush,
                ChatChannel.Ress => R.Brushes.GreenBrush,
                ChatChannel.Quest => R.Brushes.ChatSystemQuestBrush,
                ChatChannel.Friend => R.Brushes.ChatSystemFriendBrush,
                ChatChannel.Twitch => R.Brushes.TwitchBrush,
                ChatChannel.WorldBoss => R.Brushes.ChatSystemWorldBossBrush,
                ChatChannel.Guardian => R.Brushes.GuardianBrush,
                _ => R.Brushes.ChatSystemGenericBrush
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
