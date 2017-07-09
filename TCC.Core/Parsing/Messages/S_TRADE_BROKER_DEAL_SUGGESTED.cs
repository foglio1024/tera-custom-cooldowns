using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_TRADE_BROKER_DEAL_SUGGESTED : ParsedMessage
    {
        public uint PlayerId { get; private set; }
        public uint Listing { get; private set; }
        public int Item { get; private set; }
        public long Amount { get; private set; }
        public long SellerPrice { get; private set; }
        public long OfferedPrice { get; private set; }
        public string Name { get; private set; }

        public S_TRADE_BROKER_DEAL_SUGGESTED(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            PlayerId = reader.ReadUInt32();
            Listing = reader.ReadUInt32();
            Item = reader.ReadInt32();
            Amount = reader.ReadInt64();
            SellerPrice= reader.ReadInt64();
            OfferedPrice = reader.ReadInt64();
            Name = reader.ReadTeraString();
        }
    }
}
