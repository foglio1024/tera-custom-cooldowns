using System.Diagnostics;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_REQUEST_CONTRACT : ParsedMessage
    {
        internal S_REQUEST_CONTRACT(TeraMessageReader reader) : base(reader)
        {
            //PrintRaw();
            reader.Skip(24);
            var type = reader.ReadInt16();
            Type = (RequestType)type;
            reader.Skip(14);
            //int unk3 = reader.ReadInt32();
            //int time = reader.ReadInt32();
            Sender = reader.ReadTeraString();
            Recipient = reader.ReadTeraString();
            Debug.WriteLine("type:"+type+";translated:"+Type+"; Sender:"+Sender+";Recipient"+Recipient);
        }

        public string Sender { get; private set; }
        public string Recipient { get; private set; }
        public enum RequestType
        {
            UnStuck=16,
            DungeonTeleporter = 15,
            Mailbox = 8,
            MapTeleporter =  14,
            TeraClubMapTeleporter = 53,
            TeraClubTravelJournalTeleporter = 54,
            OpenBox = 43,
            LootBox = 52,
            ChooseLootDialog = 20, //(aka: goldfinger + elion token + ...)
            BankOpen = 26,
            TeraClubDarkanFlameUse = 33, // or merge multiple item together
            PartyInvite = 4,
            TradeRequest = 3,
            ShopOpen = 9,
            Craft = 31
        }

        public RequestType Type { get; private set; }
    }
}

