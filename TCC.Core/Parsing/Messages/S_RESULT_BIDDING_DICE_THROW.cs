using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_RESULT_BIDDING_DICE_THROW : ParsedMessage
    {
        public ulong EntityId { get; private set; }
        public int RollResult { get; private set; }
        public S_RESULT_BIDDING_DICE_THROW(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadUInt64();
            RollResult = reader.ReadInt32();
        }
    }
}
