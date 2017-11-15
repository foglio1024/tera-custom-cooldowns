using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_CHANGE_HP : ParsedMessage
    {
        uint serverId, playerId;
        int currentHP, maxHP;
        public uint ServerId { get { return serverId; } }
        public uint PlayerId { get { return playerId; } }
        public int CurrentHP { get { return currentHP; } }
        public int MaxHP { get { return maxHP; } }
        public S_PARTY_MEMBER_CHANGE_HP(TeraMessageReader reader) : base(reader)
        {
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();

            currentHP = reader.ReadInt32();
            if (reader.Version < 321550 || reader.Version > 321600) reader.Skip(4);
            maxHP = reader.ReadInt32();
        }
    }
}