


namespace TeraPacketParser.Messages
{
    public class S_ABNORMALITY_REFRESH : ParsedMessage
    {
        public ulong TargetId { get ; }
        public uint AbnormalityId { get; }
        public uint Duration { get ; }
        public int Stacks { get; }

        public S_ABNORMALITY_REFRESH(TeraMessageReader reader) : base(reader)
        {
            TargetId= reader.ReadUInt64();
            AbnormalityId= reader.ReadUInt32();
            Duration= reader.ReadUInt32();
            reader.Skip(4);
            Stacks= reader.ReadInt32();
        }
    }
}
