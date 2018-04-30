using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_CHANGE_MP : ParsedMessage
    {
        private uint serverId, playerId;
        private int currentMP, maxMP;
        public uint ServerId { get { return serverId; } }
        public uint PlayerId { get { return playerId; } }
        public int CurrentMP { get { return currentMP; } }
        public int MaxMP { get { return maxMP; } }

        public S_PARTY_MEMBER_CHANGE_MP(TeraMessageReader reader) : base(reader)
        {
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();

            currentMP = reader.ReadInt32();
            maxMP = reader.ReadInt32();

        }
    }
}