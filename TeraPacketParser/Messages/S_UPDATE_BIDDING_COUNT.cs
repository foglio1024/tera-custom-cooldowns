namespace TeraPacketParser.Messages;

public class S_UPDATE_BIDDING_COUNT : ParsedMessage
{
    public int Count { get; }

    public S_UPDATE_BIDDING_COUNT(TeraMessageReader reader) : base(reader)
    {
        Count = reader.ReadInt32();
    }
}