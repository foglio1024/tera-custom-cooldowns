


namespace TeraPacketParser.Messages;

public class S_PARTY_MEMBER_INTERVAL_POS_UPDATE : ParsedMessage
{
    public uint ServerId { get; private set; }
    public uint PlayerId { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Z { get; private set; }
    public uint ContinentId { get; private set; }
    public int Channel { get; private set; }

    public S_PARTY_MEMBER_INTERVAL_POS_UPDATE(TeraMessageReader reader) : base(reader)
    {
        ServerId = reader.ReadUInt32();
        PlayerId = reader.ReadUInt32();
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
        ContinentId = reader.ReadUInt32();
        Channel = reader.ReadInt32();
    }
}