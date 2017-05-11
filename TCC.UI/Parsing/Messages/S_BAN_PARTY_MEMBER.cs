using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_BAN_PARTY_MEMBER : ParsedMessage
    {
        uint serverId, playerId;
        string name;

        public uint ServerId { get { return serverId; } }
        public uint PlayerId { get { return playerId; } }
        public string Name { get { return name; } }

        public S_BAN_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            reader.Skip(4); //0xFFFFFFFF
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();
            name = reader.ReadTeraString();
        }

    }
}
