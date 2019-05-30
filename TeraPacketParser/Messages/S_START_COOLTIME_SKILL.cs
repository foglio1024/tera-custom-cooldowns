namespace TeraPacketParser.Messages
{
    public class S_START_COOLTIME_SKILL : ParsedMessage
    {
        public uint SkillId { get; private set; }
        public uint Cooldown { get; private set; }

        public S_START_COOLTIME_SKILL(TeraMessageReader reader) : base(reader)
        {
            SkillId = reader.Factory.ReleaseVersion < 74 ? reader.ReadUInt32() - 0x04000000 : (uint)new SkillId(reader).Id;
            Cooldown = reader.ReadUInt32();
        }
    }
}
