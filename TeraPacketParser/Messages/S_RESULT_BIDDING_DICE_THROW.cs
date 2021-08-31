


namespace TeraPacketParser.Messages
{
    public class S_RESULT_BIDDING_DICE_THROW : ParsedMessage
    {
        public ulong EntityId { get; }
        public uint ServerId { get; }
        public uint PlayerId { get; }
        public int RollResult { get; }
        public S_RESULT_BIDDING_DICE_THROW(TeraMessageReader reader) : base(reader)
        {
            if (reader.Factory.ReleaseVersion / 100 >= 108)
            {
                ServerId = reader.ReadUInt32();
                PlayerId = reader.ReadUInt32();
            }
            else
            {
                EntityId = reader.ReadUInt64();
            }
            RollResult = reader.ReadInt32();
        }
    }
}
