namespace TeraPacketParser.Messages;

public class S_SPAWN_EVENT_SEED: ParsedMessage
{
    public GameId GameId { get; }
    public S_SPAWN_EVENT_SEED(TeraMessageReader reader) : base(reader)
    {
        GameId = GameId.Parse(reader.ReadUInt64());
    }
}