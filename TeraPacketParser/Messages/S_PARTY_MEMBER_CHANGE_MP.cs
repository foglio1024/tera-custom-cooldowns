


namespace TeraPacketParser.Messages;

public class S_PARTY_MEMBER_CHANGE_MP : ParsedMessage
{
    public uint ServerId { get; }

    public uint PlayerId { get; }

    public int CurrentMP { get; }

    public int MaxMP { get; }

    public S_PARTY_MEMBER_CHANGE_MP(TeraMessageReader reader) : base(reader)
    {
        ServerId = reader.ReadUInt32();
        PlayerId = reader.ReadUInt32();

        CurrentMP = reader.ReadInt32();
        MaxMP = reader.ReadInt32();

    }
}