


namespace TeraPacketParser.Messages;

public class S_PARTY_MEMBER_CHANGE_HP : ParsedMessage
{
    uint _serverId, _playerId;
    int _currentHP, _maxHP;
    public uint ServerId => _serverId;
    public uint PlayerId => _playerId;
    public int CurrentHP => _currentHP;
    public int MaxHP => _maxHP;

    public S_PARTY_MEMBER_CHANGE_HP(TeraMessageReader reader) : base(reader)
    {
        _serverId = reader.ReadUInt32();
        _playerId = reader.ReadUInt32();

        _currentHP = reader.ReadInt32();
        //if (reader.Version < 321550 || reader.Version > 321600)
        reader.Skip(4);
        _maxHP = reader.ReadInt32();
    }
}