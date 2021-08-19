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

        //public bool InCombat { get; }
        public S_USER_STATUS.UserStatus Status { get; }

        public short Vitality { get; }

        public bool Alive { get; }

        public int CurrentStamina {  get; }

        public int MaxStamina {  get; }

        public GroupMemberData GroupMemberData { get; }

        public S_PARTY_MEMBER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();

            CurrentHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            CurrentMP = reader.ReadInt32();
            MaxHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            MaxMP = reader.ReadInt32();

            Level = reader.ReadInt16();
            Status = (S_USER_STATUS.UserStatus)reader.ReadUInt16();
            Vitality = reader.ReadInt16();

            Alive = reader.ReadBoolean();

            reader.Skip(4);
            CurrentStamina = reader.ReadInt32();
            MaxStamina = reader.ReadInt32();

            GroupMemberData = new GroupMemberData()
            {
                ServerId = ServerId,
                PlayerId = PlayerId,
                CurrentHP = CurrentHP,
                CurrentMP = CurrentMP,
                CurrentST = CurrentStamina,
                MaxHP = MaxHP,
                MaxMP = MaxMP,
                MaxST = MaxStamina,
                Level = (uint)Level,
                InCombat = Status == S_USER_STATUS.UserStatus.InCombat,
                Alive = Alive
            };
        }
    }
}
