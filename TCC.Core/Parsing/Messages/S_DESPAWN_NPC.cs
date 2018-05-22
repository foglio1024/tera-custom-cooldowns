using TCC.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_DESPAWN_NPC : ParsedMessage
    {
        public ulong Target { get; private set; }
        public DespawnType Type { get; private set; }

        public S_DESPAWN_NPC(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            reader.Skip(3*4);
            Type = (DespawnType) reader.ReadUInt32();
        }
    }
}
