using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_LEAVE_PRIVATE_CHANNEL : ParsedMessage
    {
        public uint Id { get; private set; }
        public S_LEAVE_PRIVATE_CHANNEL(TeraMessageReader reader) : base(reader)
        {
            Id = reader.ReadUInt32();
        }
    }
}
