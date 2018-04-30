using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    internal class S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA : ParsedMessage
    {
        public uint PlayerId { get; set; }
        public uint WorldId { get; set; }
        public uint GuardId { get; set; }
        public uint SectionId { get; set; }
        public S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA(TeraMessageReader reader) : base(reader)
        {
            PlayerId = reader.ReadUInt32();
            WorldId = reader.ReadUInt32();
            GuardId = reader.ReadUInt32();
            SectionId = reader.ReadUInt32();
        }
    }
}
