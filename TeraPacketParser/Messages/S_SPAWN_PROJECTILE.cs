namespace TeraPacketParser.Messages;

public class S_SPAWN_PROJECTILE : ParsedMessage
{
    public GameId GameId { get; }
    public S_SPAWN_PROJECTILE(TeraMessageReader reader) : base(reader)
    {
        GameId = GameId.Parse(reader.ReadUInt64());
    }
}