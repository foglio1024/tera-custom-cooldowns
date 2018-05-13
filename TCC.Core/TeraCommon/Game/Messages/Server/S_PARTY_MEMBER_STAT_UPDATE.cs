using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_PARTY_MEMBER_STAT_UPDATE : ParsedMessage
    {
        internal S_PARTY_MEMBER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            HpRemaining = reader.ReadInt32();
            MpRemaining = reader.ReadInt32();
            TotalHp = reader.ReadInt32();
            TotalMp = reader.ReadInt32();
            Level = reader.ReadInt16();
            InCombat = reader.ReadInt16();
            Vitality = reader.ReadInt16();
            Alive = reader.ReadByte(); //not sure
            Stamina = reader.ReadInt32();
            ReRemaining = reader.ReadInt32();
            TotalRe = reader.ReadInt32();
            Unknow3 = reader.ReadInt32();

            //Debug.WriteLine("target = " + PlayerId + ";Hp left:" + HpRemaining + ";Max HP:" + TotalHp + ";Unknow3:" + Unknow3);
        }

        public int Unknow3 { get; }

        public int HpRemaining { get; }
        public int MpRemaining { get; }
        public int ReRemaining { get; }

        public int TotalHp { get; }
        public int TotalMp { get; }
        public int TotalRe { get; }

        public int Stamina { get; }

        public int Level { get; }
        public int InCombat { get; }
        public int Vitality { get; }
        public byte Alive { get; }

        public uint ServerId { get; }
        public uint PlayerId { get; }
        public bool Slaying => TotalHp > HpRemaining*2 && HpRemaining > 0;
    }
}