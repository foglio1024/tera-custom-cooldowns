using Tera.Game;
using Tera.Game.Messages;

namespace TCC
{
    public class S_PLAYER_CHANGE_MP : ParsedMessage
    {
        public int currentMP, maxMP, diff;
        public uint type;
        public ulong target, source;

        public S_PLAYER_CHANGE_MP(TeraMessageReader reader) : base(reader)
        {
            currentMP = reader.ReadInt32();
            maxMP = reader.ReadInt32();
            diff = reader.ReadInt32();
            type = reader.ReadUInt32();
            target = reader.ReadUInt64();
            source = reader.ReadUInt64();
        }
    }
}