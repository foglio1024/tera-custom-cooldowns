using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_LEAVE_PARTY_MEMBER : ParsedMessage
    {
        private uint serverId;
        public uint ServerId => serverId;

        private uint playerId;
        public uint PlayerId => playerId;

        private string name;
        public string Name => name;

        public S_LEAVE_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2); //var nameOffset = reader.ReadUInt16();
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();
            name = reader.ReadTeraString();
        }
    }
}
