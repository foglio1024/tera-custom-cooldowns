using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_CHANGE_HP : ParsedMessage
    {
        private uint serverId, playerId;
        private int currentHP, maxHP;
        public uint ServerId => serverId;
        public uint PlayerId => playerId;
        public int CurrentHP => currentHP;
        public int MaxHP => maxHP;

        public S_PARTY_MEMBER_CHANGE_HP(TeraMessageReader reader) : base(reader)
        {
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();

            currentHP = reader.ReadInt32();
            //if (reader.Version < 321550 || reader.Version > 321600)
                reader.Skip(4);
            maxHP = reader.ReadInt32();
        }
    }
}