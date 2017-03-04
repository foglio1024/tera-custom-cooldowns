using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.UI.Messages
{
    class S_PLAYER_STAT_UPDATE : ParsedMessage
    {
        public int edge;

        public S_PLAYER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(199);
            edge = reader.ReadUInt16();
        }
    }
}
