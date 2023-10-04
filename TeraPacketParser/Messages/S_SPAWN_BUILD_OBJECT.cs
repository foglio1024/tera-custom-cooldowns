namespace TeraPacketParser.Messages;

public class S_SPAWN_BUILD_OBJECT : ParsedMessage
{
    public GameId GameId { get; }
    public S_SPAWN_BUILD_OBJECT(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(8);
        GameId = GameId.Parse(reader.ReadUInt64());
    }
}