


namespace TeraPacketParser.Messages;

public class S_LEAVE_PARTY_MEMBER : ParsedMessage
{
    private uint _serverId;
    public uint ServerId => _serverId;

    private uint _playerId;
    public uint PlayerId => _playerId;

    private string _name;
    public string Name => _name;

    public S_LEAVE_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(2); //var nameOffset = reader.ReadUInt16();
        _serverId = reader.ReadUInt32();
        _playerId = reader.ReadUInt32();
        _name = reader.ReadTeraString();
    }
}