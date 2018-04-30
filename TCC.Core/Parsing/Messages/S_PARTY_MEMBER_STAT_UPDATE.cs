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

        public uint ServerId { get { return serverId; } }
        public uint PlayerId { get { return playerId; } }
        public long CurrentHP { get { return curHP; } }
        public int CurrentMP { get { return curMP; } }
        public long MaxHP { get { return maxHP; } }
        public int MaxMP { get { return maxMP; } }
        public short Level { get { return level; } }
        public short Combat { get { return combat; } }
        public short Vitality { get { return vitality; } }
        public bool Alive { get { return alive; } }
        public int CurrentRE { get { return curRE; } }
        public int MaxRE { get { return maxRE; } }

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
