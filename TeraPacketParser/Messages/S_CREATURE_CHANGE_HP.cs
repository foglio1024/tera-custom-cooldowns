
namespace TeraPacketParser.Messages;

public class S_CREATURE_CHANGE_HP : ParsedMessage
{
    public long CurrentHP { get; }
    public long MaxHP { get; }
    public long Diff { get; }
    public ulong Target { get; }
    public ulong Source { get; }
    public bool Crit { get; } 

    public S_CREATURE_CHANGE_HP(TeraMessageReader reader) : base(reader)
    {
        CurrentHP = reader.ReadInt64();
        MaxHP = reader.ReadInt64();
        Diff = reader.ReadInt64();
        reader.Skip(4);
        Target = reader.ReadUInt64();
        Source = reader.ReadUInt64();
        Crit = reader.ReadBoolean();
    }
}