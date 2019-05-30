


namespace TeraPacketParser.Messages
{
    public class S_START_COOLTIME_ITEM : ParsedMessage
    {
        /// <summary>
        /// Item's ID
        /// </summary>
        public uint ItemId { get; private set; }

        /// <summary>
        /// Item's cooldown in seconds
        /// </summary>
        public uint Cooldown { get; private set; }

        public S_START_COOLTIME_ITEM(TeraMessageReader reader) : base(reader)
        {
            ItemId = reader.ReadUInt32(); //- 0x04000000;
            Cooldown = reader.ReadUInt32();
        }
    }
}
