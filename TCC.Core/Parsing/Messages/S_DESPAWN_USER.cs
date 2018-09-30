using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_DESPAWN_USER : ParsedMessage
    {
        public ulong EntityId { get; }

        public S_DESPAWN_USER(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadUInt64();
        }
    }
}