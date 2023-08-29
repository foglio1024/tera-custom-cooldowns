namespace TeraPacketParser.Messages
{
    public class S_SPAWN_COLLECTION: ParsedMessage
    {
        public GameId GameId { get; }
        public S_SPAWN_COLLECTION(TeraMessageReader reader) : base(reader)
        {
            GameId = GameId.Parse(reader.ReadUInt64());
        }
    }

}