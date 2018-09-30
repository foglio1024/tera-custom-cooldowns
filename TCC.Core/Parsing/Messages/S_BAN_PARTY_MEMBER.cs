using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_BAN_PARTY_MEMBER : ParsedMessage
    {
        private uint serverId, playerId;
        private string name;

        public uint ServerId => serverId;
        public uint PlayerId => playerId;
        public string Name => name;

        public S_BAN_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
        {
            //var nameOffset = reader.ReadUInt16();
            reader.Skip(2);
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();
            reader.Skip(4); //0xFFFFFFFF
            name = reader.ReadTeraString();
        }

    }
}
