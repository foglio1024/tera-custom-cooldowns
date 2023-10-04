namespace TeraPacketParser.Messages;

public enum RunemarksActionType
{
    Normal = 0,
    Detonate = 1,
    Expired = 2,
    Reclaimed = 3
}
public class S_WEAK_POINT : ParsedMessage
{
    public ulong Target { get; }
    public int TotalRunemarks { get; }
    public uint RemovedRunemarks { get; }
    public RunemarksActionType Type { get; }
    public uint SkillId { get; }

    public S_WEAK_POINT(TeraMessageReader reader) : base(reader)
    {
        Target = reader.ReadUInt64();
        RemovedRunemarks = reader.ReadUInt32();
        TotalRunemarks = reader.ReadInt32();
        Type = (RunemarksActionType)reader.ReadUInt32();
        SkillId = reader.ReadUInt32();
    }

    public override string ToString()
    {
        return $"T:{Target} | total:{TotalRunemarks} | moved:{RemovedRunemarks} | {Type} | S:{SkillId}";
    }
}