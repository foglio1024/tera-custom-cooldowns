


namespace TeraPacketParser.Messages;

public class S_LOGOUT_PARTY_MEMBER : ParsedMessage
{
    uint _serverId;
    public uint ServerId => _serverId;

    uint _playerId;
    public uint PlayerId => _playerId;

    public S_LOGOUT_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
    {
        _serverId = reader.ReadUInt32();
        _playerId = reader.ReadUInt32();
    }
}