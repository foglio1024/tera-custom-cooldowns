using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class C_LOAD_TOPO_FIN : ParsedMessage
    {
        public C_LOAD_TOPO_FIN(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
