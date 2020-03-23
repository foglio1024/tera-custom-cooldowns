using TeraPacketParser.Data;

namespace TeraPacketParser.Messages
{
    public class S_START_COOLTIME_SKILL : ParsedMessage
    {
        public uint SkillId { get; private set; }
        public uint Cooldown { get; private set; }

        public S_START_COOLTIME_SKILL(TeraMessageReader reader) : base(reader)
        {
            SkillId =  (uint)new SkillId(reader).Id;
            Cooldown = reader.ReadUInt32();
        }
    }
}
