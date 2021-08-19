namespace TeraPacketParser.Messages
{
    public class S_WHISPER : ParsedMessage
    {
        public ulong GameId { get; }
        public string Author { get; }
        public string Recipient { get; }
        public string Message { get; }
        public bool IsGm { get; }
        public S_WHISPER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(6);
            GameId = reader.ReadUInt64();
            if(reader.Factory.ReleaseVersion /100 >= 108)
            {
                reader.Skip(8); // senderServerId + recipientServerId
            }
            reader.Skip(1); // isWorldEventTarget
            IsGm = reader.ReadBoolean();
            reader.Skip(1); // isFounder
            Author = reader.ReadTeraString();
            Recipient = reader.ReadTeraString();
            Message = reader.ReadTeraString();
        }
    }
}
