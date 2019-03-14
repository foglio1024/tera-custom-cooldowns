using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_LOGOUT_PARTY_MEMBER : ParsedMessage
    {
        private uint _serverId;
        public uint ServerId => _serverId;

        private uint _playerId;
        public uint PlayerId => _playerId;

        public S_LOGOUT_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
        {
            _serverId = reader.ReadUInt32();
            _playerId = reader.ReadUInt32();
        }
    }
}