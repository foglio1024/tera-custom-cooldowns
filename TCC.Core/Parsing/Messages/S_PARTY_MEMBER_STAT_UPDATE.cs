using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_STAT_UPDATE : ParsedMessage
    {
        private uint serverId, playerId;
        private int curMP, maxMP;
        private long curHP, maxHP;
        private short level, combat, vitality;
        private bool alive;
        private int stamina, curRE, maxRE;

        public uint ServerId => serverId;
        public uint PlayerId => playerId;
        public long CurrentHP => curHP;
        public int CurrentMP => curMP;
        public long MaxHP => maxHP;
        public int MaxMP => maxMP;
        public short Level => level;
        public short Combat => combat;
        public short Vitality => vitality;
        public bool Alive => alive;
        public int CurrentRE => curRE;
        public int MaxRE => maxRE;

        public S_PARTY_MEMBER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();

            curHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            curMP = reader.ReadInt32();
            maxHP = /*reader.Version < 321550 || reader.Version > 321600 ? */reader.ReadInt64() /*: reader.ReadInt32()*/;
            maxMP = reader.ReadInt32();

            level = reader.ReadInt16();
            combat = reader.ReadInt16();
            vitality = reader.ReadInt16();

            alive = reader.ReadBoolean();

            stamina = reader.ReadInt32();
            curRE = reader.ReadInt32();
            maxRE = reader.ReadInt32();
        }
    }
}
