using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_ME : ParsedMessage
    {
        public S_SPAWN_ME(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
