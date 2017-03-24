using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    public class S_GET_USER_LIST : ParsedMessage
    {
        public S_GET_USER_LIST(TeraMessageReader reader) : base(reader)
        {

        }
    }
}
