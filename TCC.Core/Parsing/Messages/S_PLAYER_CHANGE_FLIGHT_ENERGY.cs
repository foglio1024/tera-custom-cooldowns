using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PLAYER_CHANGE_FLIGHT_ENERGY : ParsedMessage
    {
        public float Energy { get; private set; }
        public S_PLAYER_CHANGE_FLIGHT_ENERGY(TeraMessageReader reader) : base(reader)
        {
            Energy = reader.ReadSingle();
        }
    }
}
