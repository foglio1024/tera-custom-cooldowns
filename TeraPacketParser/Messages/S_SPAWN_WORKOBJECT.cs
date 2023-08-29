namespace TeraPacketParser.Messages
{
    public class S_SPAWN_WORKOBJECT : ParsedMessage
    {
        public GameId GameId { get; }
        public S_SPAWN_WORKOBJECT(TeraMessageReader reader) : base(reader)
        {
            GameId = GameId.Parse(reader.ReadUInt64());
        }
    }

}