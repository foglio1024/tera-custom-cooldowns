using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_RETURN_TO_LOBBY : ParsedMessage
    {
        public S_RETURN_TO_LOBBY(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
