using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_CHANGE_MP : ParsedMessage
    {
        private uint _serverId, _playerId;
        private int _currentMP, _maxMP;
        public uint ServerId => _serverId;
        public uint PlayerId => _playerId;
        public int CurrentMP => _currentMP;
        public int MaxMP => _maxMP;

        public S_PARTY_MEMBER_CHANGE_MP(TeraMessageReader reader) : base(reader)
        {
            _serverId = reader.ReadUInt32();
            _playerId = reader.ReadUInt32();

            _currentMP = reader.ReadInt32();
            _maxMP = reader.ReadInt32();

        }
    }
}