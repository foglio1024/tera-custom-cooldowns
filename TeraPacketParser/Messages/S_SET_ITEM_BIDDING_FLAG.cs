namespace TeraPacketParser.Messages;

public class S_SET_ITEM_BIDDING_FLAG : ParsedMessage
{
    public GameId GameId { get; }
    public bool Flag { get; }

    internal S_SET_ITEM_BIDDING_FLAG(TeraMessageReader reader) : base(reader)
    {
        GameId = GameId.Parse(reader.ReadUInt64());
        Flag = reader.ReadBoolean();
    }
}