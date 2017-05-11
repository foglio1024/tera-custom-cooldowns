using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_STAT_UPDATE : ParsedMessage
    {
        private uint serverId, playerId;
        int curHP, curMP, maxHP, maxMP;
        short level, combat, vitality;
        bool alive;
        int stamina, curRE, maxRE;

        public uint ServerId { get { return serverId; } }
        public uint PlayerId { get { return playerId; } }
        public int CurrentHP { get { return curHP; } }
        public int CurrentMP { get { return curMP; } }
        public int MaxHP { get { return maxHP; } }
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

            curHP = reader.ReadInt32();
            curMP = reader.ReadInt32();
            maxHP = reader.ReadInt32();
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
