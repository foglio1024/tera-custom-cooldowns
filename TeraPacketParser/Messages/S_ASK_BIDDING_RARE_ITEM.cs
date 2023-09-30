namespace TeraPacketParser.Messages;

public class S_ASK_BIDDING_RARE_ITEM : ParsedMessage
{
    public int Index { get; }
    public uint Amount { get; }
    public uint ItemId { get; }

    public S_ASK_BIDDING_RARE_ITEM(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(4);
        Index = reader.ReadInt32();
        reader.Skip(4);
        ItemId = reader.ReadUInt32();
        Amount = reader.ReadUInt32();
    }
}