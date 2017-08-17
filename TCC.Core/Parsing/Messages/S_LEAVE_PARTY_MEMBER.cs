using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_LEAVE_PARTY_MEMBER : ParsedMessage
    {
        private uint serverId;
        public uint ServerId
        {
            get { return serverId; }
        }

        private uint playerId;
        public uint PlayerId
        {
            get { return playerId; }

        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        public S_LEAVE_PARTY_MEMBER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2); //var nameOffset = reader.ReadUInt16();
            serverId = reader.ReadUInt32();
            playerId = reader.ReadUInt32();
            name = reader.ReadTeraString();
        }
    }
}
