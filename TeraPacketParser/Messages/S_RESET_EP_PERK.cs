


namespace TeraPacketParser.Messages;

public class S_RESET_EP_PERK : ParsedMessage
{
    public bool Success { get; }
    public S_RESET_EP_PERK(TeraMessageReader r) : base(r)
    {
        Success = r.ReadBoolean();
    }
}