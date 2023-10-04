namespace TeraPacketParser.Data;

public struct SkillId
{
    readonly ulong _raw;

    public SkillId(TeraMessageReader reader) : this()
    {
        _raw = reader.ReadUInt64();
    }

    public int Id => (int)(_raw & 0x00000000_0FFFFFFF);
    public bool IsAction => (_raw & 0x00000000_10000000) != 0;
    public bool IsReaction => (_raw & 0x00000000_20000000) != 0;
    bool HasHuntingZone => (_raw & 0x00000001_00000000) != 0;
    public int HuntingZone => HasHuntingZone ? (int)((_raw & 0x00000000_0FFF0000) >> 16) : 0;
}