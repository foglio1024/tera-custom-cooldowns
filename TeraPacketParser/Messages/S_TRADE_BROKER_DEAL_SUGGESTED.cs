namespace TeraPacketParser.Messages;

public class S_TRADE_BROKER_DEAL_SUGGESTED : ParsedMessage
{
    public uint PlayerId { get; }
    public uint Listing { get; }
    public int Item { get; }
    public long Amount { get; }
    public long SellerPrice { get; }
    public long OfferedPrice { get; }
    public string Name { get; }

    public S_TRADE_BROKER_DEAL_SUGGESTED(TeraMessageReader reader) : base(reader)
    {
        var nameOffset = reader.ReadUInt16();
        PlayerId = reader.ReadUInt32();
        Listing = reader.ReadUInt32();
        Item = reader.ReadInt32();
        Amount = reader.ReadInt64();
        SellerPrice = reader.ReadInt64();
        OfferedPrice = reader.ReadInt64();
        try
        {
            reader.RepositionAt(nameOffset);
            Name = reader.ReadTeraString();
        }
        catch
        {
            Name = "";
        }
    }
}