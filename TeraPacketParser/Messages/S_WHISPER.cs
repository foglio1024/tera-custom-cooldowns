


namespace TeraPacketParser.Messages
{
    public class S_WHISPER : ParsedMessage
    {
        public ulong GameId { get; private set; }
        public string Author { get; private set; }
        public string Recipient { get; private set; }
        public string Message { get; private set; }
        public S_WHISPER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(6);
            GameId = reader.ReadUInt64();
            reader.Skip(3);
            Author = reader.ReadTeraString();
            Recipient = reader.ReadTeraString();
            Message = reader.ReadTeraString();
        }
    }
}
