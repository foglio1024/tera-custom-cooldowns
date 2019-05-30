namespace TeraPacketParser.Messages
{
    public class S_DECREASE_COOLTIME_SKILL : ParsedMessage
    {
        /// <summary>
        /// Skill's ID
        /// </summary>
        public uint SkillId { get; private set; }

        /// <summary>
        /// Skill's cooldown in milliseconds
        /// </summary>
        public uint Cooldown { get; private set; }

        public S_DECREASE_COOLTIME_SKILL(TeraMessageReader reader) : base(reader)
        {
            SkillId = reader.Factory.ReleaseVersion < 74 ? reader.ReadUInt32() - 0x04000000 : (uint)new SkillId(reader).Id;
            Cooldown = reader.ReadUInt32();
        }

    }
}
