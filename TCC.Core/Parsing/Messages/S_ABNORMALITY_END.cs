using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_ABNORMALITY_END : ParsedMessage
    {
        public ulong Target { get; private set; }
        public uint Id { get; private set; }

        public S_ABNORMALITY_END(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            Id = reader.ReadUInt32();
        }
    }
}
