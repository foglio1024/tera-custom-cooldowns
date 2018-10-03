using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_SHOW_HP : ParsedMessage
    {
        public ulong GameId { get; }
        public long CurrentHp { get; }
        public long MaxHp { get; }
        public bool Enemy { get; }
        public S_SHOW_HP(TeraMessageReader reader) : base(reader)
        {
            GameId = reader.ReadUInt64();
            CurrentHp = reader.ReadInt64();
            MaxHp = reader.ReadInt64();
            Enemy = reader.ReadBoolean();
        }
    }
}
