


namespace TeraPacketParser.Messages
{
    public class S_CHAT : ParsedMessage
    {
        public uint Channel { get; }
        public ulong GameId { get; }
        public string Name { get; }
        public string Message { get; }
        public bool IsGm { get; }

        public S_CHAT(TeraMessageReader reader) : base(reader)
        {
            var authorNameOffset = reader.ReadUInt16();
            var messageOffset = reader.ReadUInt16();
            Channel = reader.ReadUInt32();
            GameId = reader.ReadUInt64();
            if(reader.Factory.ReleaseVersion /100 >= 108)
            {
                reader.Skip(8); // uint32 playerId + uint32 serverId
            }
            reader.Skip(1);
            IsGm = reader.ReadBoolean();
            reader.Skip(1);
            reader.BaseStream.Position = authorNameOffset - 4;
            Name= reader.ReadTeraString();
            reader.BaseStream.Position = messageOffset - 4;
            Message = reader.ReadTeraString();
            
        }
    }
}
