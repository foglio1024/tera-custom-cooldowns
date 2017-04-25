using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_LOAD_TOPO : ParsedMessage
    {
        public S_LOAD_TOPO(TeraMessageReader reader) : base(reader)
        {

        }
    }
}
