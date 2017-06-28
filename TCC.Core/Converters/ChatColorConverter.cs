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
                    return ChatWindowBrushes.Say;
                case ChatChannel.Party:
                    return ChatWindowBrushes.Party;
                case ChatChannel.Guild:
                    return ChatWindowBrushes.Guild;
                case ChatChannel.Area:
                    return ChatWindowBrushes.Area;
                case ChatChannel.Trade:
                    return ChatWindowBrushes.Trade;
                case ChatChannel.Greet:
                    return ChatWindowBrushes.Greet;
                case ChatChannel.PartyNotice:
                    return ChatWindowBrushes.PartyNotice;
                case ChatChannel.RaidNotice:
                    return ChatWindowBrushes.RaidNotice;
                case ChatChannel.Emote:
                    return ChatWindowBrushes.Emote;
                case ChatChannel.Global:
                    return ChatWindowBrushes.Global;
                case ChatChannel.Raid:
                    return ChatWindowBrushes.Raid;
                case ChatChannel.Megaphone:
                    return ChatWindowBrushes.Megaphone;
                case ChatChannel.GuildAdvertising:
                    return ChatWindowBrushes.GuildAd;
                case ChatChannel.Private1:
                    return ChatWindowBrushes.Private;
                case ChatChannel.Private2:
                    return ChatWindowBrushes.Private;
                case ChatChannel.Private3:
                    return ChatWindowBrushes.Private;
                case ChatChannel.Private4:
                    return ChatWindowBrushes.Private;
                case ChatChannel.Private5:
                    return ChatWindowBrushes.Private;
                case ChatChannel.Private6:
                    return ChatWindowBrushes.Private;
                case ChatChannel.Private7:
                    return ChatWindowBrushes.Private;
                case ChatChannel.Private8:
                    return ChatWindowBrushes.Private;
                case ChatChannel.SentWhisper:
                    return ChatWindowBrushes.Whisper;
                case ChatChannel.ReceivedWhisper:
                    return ChatWindowBrushes.Whisper;
                case ChatChannel.System:
                    return ChatWindowBrushes.SystemGeneric;
                case ChatChannel.Notify:
                    return ChatWindowBrushes.SystemNotify;
                case ChatChannel.Event:
                    return ChatWindowBrushes.SystemEvent;
                case ChatChannel.Error:
                    return ChatWindowBrushes.SystemError;
                case ChatChannel.Group:
                    return ChatWindowBrushes.SystemGroup;
                case ChatChannel.GuildNotice:
                    return ChatWindowBrushes.Guild;
                case ChatChannel.Deathmatch:
                    return ChatWindowBrushes.SystemGroup;
                case ChatChannel.ContractAlert:
                    return ChatWindowBrushes.SystemGeneric;
                case ChatChannel.GroupAlerts:
                    return ChatWindowBrushes.SystemGroup;
                case ChatChannel.Loot:
                    return ChatWindowBrushes.SystemGeneric;
                case ChatChannel.Exp:
                    return ChatWindowBrushes.SystemGeneric;
                case ChatChannel.Money:
                    return ChatWindowBrushes.SystemGeneric;
                case ChatChannel.TradeRedirect:
                    return ChatWindowBrushes.Trade;
                default:
                    return ChatWindowBrushes.SystemGeneric;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
