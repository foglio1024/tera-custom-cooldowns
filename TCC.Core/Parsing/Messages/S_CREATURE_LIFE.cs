using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_CREATURE_LIFE : ParsedMessage
    {
        internal S_CREATURE_LIFE(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            var pos = reader.ReadVector3f();
            Alive = reader.ReadBoolean(); // 0=dead;1=alive
        }

        public ulong Target { get; }
        public bool Alive { get; }
    }
}