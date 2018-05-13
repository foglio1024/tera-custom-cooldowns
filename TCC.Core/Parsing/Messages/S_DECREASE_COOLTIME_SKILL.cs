using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
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
            SkillId = reader.ReadUInt32() - 0x04000000;
            Cooldown = reader.ReadUInt32();
        }

    }
}
