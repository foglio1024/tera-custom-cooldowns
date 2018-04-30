using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_ABNORMALITY_DAMAGE_ABSORB : ParsedMessage
    {
        public ulong Target { get; }
        public uint Damage { get; }
        public S_ABNORMALITY_DAMAGE_ABSORB(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            Damage = reader.ReadUInt32();
        }
    }
}
