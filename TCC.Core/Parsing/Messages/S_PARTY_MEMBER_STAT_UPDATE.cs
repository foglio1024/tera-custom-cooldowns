using TCC.Annotations;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_STAT_UPDATE : ParsedMessage
    {
        public uint ServerId { get; }

        public uint PlayerId { get; }

        public long CurrentHP { get; }

        public int CurrentMP { get; }

        public long MaxHP { get; }

        public int MaxMP { get; }

        public short Level { get; }

        public short Combat { get; }

        public short Vitality { get; }

        public bool Alive { get; }

        public int CurrentRE { [UsedImplicitly] get; }

        public int MaxRE { [UsedImplicitly] get; }

        public S_PARTY_MEMBER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();

            CurrentHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            CurrentMP = reader.ReadInt32();
            MaxHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            MaxMP = reader.ReadInt32();

            Level = reader.ReadInt16();
            Combat = reader.ReadInt16();
            Vitality = reader.ReadInt16();

            Alive = reader.ReadBoolean();

            reader.Skip(4);
            CurrentRE = reader.ReadInt32();
            MaxRE = reader.ReadInt32();
        }
    }
}
