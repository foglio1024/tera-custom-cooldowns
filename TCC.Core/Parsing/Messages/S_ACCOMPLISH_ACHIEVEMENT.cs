using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_ACCOMPLISH_ACHIEVEMENT : ParsedMessage
    {
        public uint AchievementId { get; private set; }
        public S_ACCOMPLISH_ACHIEVEMENT(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(16);
            AchievementId = reader.ReadUInt32();
        }
    }
}
