using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_CHECK_TO_READY_PARTY_FIN : ParsedMessage
    {
        public S_CHECK_TO_READY_PARTY_FIN(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
