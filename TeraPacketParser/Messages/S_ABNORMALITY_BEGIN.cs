


namespace TeraPacketParser.Messages
{
    public class S_ABNORMALITY_BEGIN : ParsedMessage
    {

        public ulong TargetId { get; private set; }
        public ulong CasterId { get; private set; }
        public uint AbnormalityId { get; private set; }
        public uint Duration { get; private set; }
        public int Stacks { get; private set; }

        public S_ABNORMALITY_BEGIN(TeraMessageReader reader) : base(reader)
        {
            if (reader.Factory.ReleaseVersion >= 10700 ||
                reader.Factory.ReleaseVersion == 9901 && reader.Factory.Version != 381995) reader.Skip(4);
            TargetId = reader.ReadUInt64();
            CasterId = reader.ReadUInt64();
            AbnormalityId = reader.ReadUInt32();
            Duration = reader.ReadUInt32();
            reader.Skip(4);
            Stacks = reader.ReadInt32();
        }
    }
}

