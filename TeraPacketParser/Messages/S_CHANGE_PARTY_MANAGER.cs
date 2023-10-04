


namespace TeraPacketParser.Messages;

public class S_CHANGE_PARTY_MANAGER : ParsedMessage
{
    public string Name { get; private set; }
    public ulong EntityId { get; private set; }
    public S_CHANGE_PARTY_MANAGER(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(2);
        EntityId = reader.ReadUInt64();
        Name = reader.ReadTeraString();
    }
}