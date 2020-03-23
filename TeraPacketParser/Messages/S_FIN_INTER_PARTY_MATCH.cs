namespace TeraPacketParser.Messages
{
    public class S_FIN_INTER_PARTY_MATCH : ParsedMessage
    {
        public int Zone { get; }
        public S_FIN_INTER_PARTY_MATCH(TeraMessageReader reader) : base(reader)
        {
            Zone = reader.ReadInt32();
        }
    }
}