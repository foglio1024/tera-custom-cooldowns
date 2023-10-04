namespace TeraPacketParser.Messages;

public class ParsedGearItem
{
    public uint ItemId { get; }
    public int Enchant { get; }
    public long Exp { get; }
    public ParsedGearItem(uint id, int en, long exp)
    {
        ItemId = id;
        Enchant = en;
        Exp = exp;
    }
}