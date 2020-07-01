namespace TeraPacketParser
{
    // Base class for parsed messages
    public abstract class ParsedMessage : Message
    {
        public ParsedMessage(TeraMessageReader reader) : base(reader.Message.Time, reader.Message.Direction, reader.Message.Data)
        {
            Raw = reader.Message.Payload.Array;
            OpCodeName = reader.OpCodeName;
        }

        public byte[]? Raw { get; }

        public string OpCodeName { get; }
    }
}