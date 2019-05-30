


namespace TeraPacketParser.Messages
{
    public class S_CHANGE_GUILD_CHIEF : ParsedMessage
    {
        public uint PlayerId { get; set; }

        public S_CHANGE_GUILD_CHIEF(TeraMessageReader reader) : base(reader)
        {
            PlayerId = reader.ReadUInt32();
        }
    }
}
