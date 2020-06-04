


namespace TeraPacketParser.Messages
{
    public class S_CHAT : ParsedMessage
    {
        public uint Channel { get; }
        public ulong AuthorId { get; }
        public string AuthorName { get; }
        public string Message { get; }
        public bool IsGm { get; }

        public S_CHAT(TeraMessageReader reader) : base(reader)
        {
            var authorNameOffset = reader.ReadUInt16();
            var messageOffset = reader.ReadUInt16();
            Channel = reader.ReadUInt32();
            AuthorId = reader.ReadUInt64();
            reader.Skip(1);
            IsGm = reader.ReadBoolean();
            reader.Skip(1);
            reader.BaseStream.Position = authorNameOffset - 4;
            AuthorName= reader.ReadTeraString();
            reader.BaseStream.Position = messageOffset - 4;
            Message = reader.ReadTeraString();
            
        }
    }
}
