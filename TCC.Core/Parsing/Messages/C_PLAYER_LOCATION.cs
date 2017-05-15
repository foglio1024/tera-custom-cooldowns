using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;

namespace TCC.Parsing.Messages
{
    public class C_PLAYER_LOCATION : Tera.Game.Messages.ParsedMessage
    {
        private float x;
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        private float y;
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public C_PLAYER_LOCATION(TeraMessageReader reader) : base(reader)
        {
            x = reader.ReadSingle();
            y = reader.ReadSingle();
        }
    }
}
