namespace TeraPacketParser.Messages;

public class S_FATIGABILITY_POINT : ParsedMessage
{
    public int CurrFatigability { get; set; }
    public int MaxFatigability { get; set; }

    public S_FATIGABILITY_POINT(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(4);
        MaxFatigability = reader.ReadInt32();
        CurrFatigability = reader.ReadInt32();
    }
}