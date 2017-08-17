using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_BAN_PARTY : ParsedMessage
    {
        public S_BAN_PARTY(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
