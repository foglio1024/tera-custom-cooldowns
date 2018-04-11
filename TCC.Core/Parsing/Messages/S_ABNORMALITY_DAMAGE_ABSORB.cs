using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_ABNORMALITY_DAMAGE_ABSORB : ParsedMessage
    {
        public ulong Target { get; }
        public uint Damage { get; }
        public S_ABNORMALITY_DAMAGE_ABSORB(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            Damage = reader.ReadUInt32();
        }
    }
}
