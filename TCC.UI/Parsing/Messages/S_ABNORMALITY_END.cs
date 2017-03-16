using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    class S_ABNORMALITY_END : ParsedMessage
    {
        public ulong target;
        public uint id;

        public S_ABNORMALITY_END(TeraMessageReader reader) : base(reader)
        {
            target = reader.ReadUInt64();
            id = reader.ReadUInt32();
        }
    }
}
