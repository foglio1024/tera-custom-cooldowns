using TCC.Annotations;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_CREATURE_CHANGE_HP : ParsedMessage
    {
        public long CurrentHP { get; }
        public long MaxHP { get; }
        public long Diff { get; }
        public ulong Target { get; }
        public ulong Source { get; }
        public byte Crit { [UsedImplicitly] get; }

        public S_CREATURE_CHANGE_HP(TeraMessageReader reader) : base(reader)
        {
            //if (reader.Version < 321550 || reader.Version > 321600)
            //{
            CurrentHP = reader.ReadInt64();
            MaxHP = reader.ReadInt64();
            Diff = reader.ReadInt64();

            //}
            //else
            //{
            //    currentHP = reader.ReadInt32();
            //    maxHP = reader.ReadInt32();
            //    diff = reader.ReadInt32();
            //}
            //type = reader.ReadUInt32();
            reader.Skip(4);
            Target = reader.ReadUInt64();
            Source = reader.ReadUInt64();
            Crit = reader.ReadByte();
        }
    }
}
