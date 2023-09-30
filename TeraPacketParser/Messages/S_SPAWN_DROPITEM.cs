namespace TeraPacketParser.Messages;

public class S_SPAWN_DROPITEM : ParsedMessage
{
    public GameId GameId { get; }
    public uint ItemId { get; }
    public uint Amount { get; }

    public S_SPAWN_DROPITEM(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(6);
        GameId = GameId.Parse(reader.ReadUInt64());
        reader.Skip(3 * 4); // loc
        ItemId = reader.ReadUInt32();
        Amount = reader.ReadUInt32();
    }
}