using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class C_REQUEST_GAMESTAT_PING : ParsedMessage
    {
        public C_REQUEST_GAMESTAT_PING(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
