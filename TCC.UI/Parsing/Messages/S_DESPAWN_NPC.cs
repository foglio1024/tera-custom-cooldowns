using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    public class S_DESPAWN_NPC : ParsedMessage
    {
        public ulong target;
        float x, y, z;
        uint type;
        int unk;
        public S_DESPAWN_NPC(TeraMessageReader reader) : base(reader)
        {
            target = reader.ReadUInt64();
        }
    }
}
