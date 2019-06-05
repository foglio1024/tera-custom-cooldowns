using TeraDataLite;

namespace TeraPacketParser.Messages
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

        public bool InCombat { get; }

        public short Vitality { get; }

        public bool Alive { get; }

        public int CurrentRE {  get; }

        public int MaxRE {  get; }

        public PartyMemberData PartyMemberData { get; }

        public S_PARTY_MEMBER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();

            CurrentHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            CurrentMP = reader.ReadInt32();
            MaxHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            MaxMP = reader.ReadInt32();

            Level = reader.ReadInt16();
            InCombat = reader.ReadInt16() > 0;
            Vitality = reader.ReadInt16();

            Alive = reader.ReadBoolean();

            reader.Skip(4);
            CurrentRE = reader.ReadInt32();
            MaxRE = reader.ReadInt32();

            PartyMemberData = new PartyMemberData()
            {
                ServerId = ServerId,
                PlayerId = PlayerId,
                CurrentHP = CurrentHP,
                CurrentMP = CurrentMP,
                CurrentST = CurrentRE,
                MaxHP = MaxHP,
                MaxMP = MaxMP,
                MaxST = MaxRE,
                Level = (uint)Level,
                InCombat = InCombat,
                Alive = Alive
            };
        }
    }
}
