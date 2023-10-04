


namespace TeraPacketParser.Messages;

public class S_BAN_PARTY_MEMBER : ParsedMessage
{
    uint _serverId, _playerId;
    string _name;

    public uint ServerId => _serverId;
    public uint PlayerId => _playerId;
    public string Name => _name;

    public S_BAN_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
    {
        //var nameOffset = reader.ReadUInt16();
        reader.Skip(2);
        _serverId = reader.ReadUInt32();
        _playerId = reader.ReadUInt32();
        reader.Skip(4); //0xFFFFFFFF
        _name = reader.ReadTeraString();
    }

}