using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{

    public class S_FIELD_POINT_INFO : ParsedMessage
    {
        public uint Points { get; }
        public uint MaxPoints { get; set; }

        public S_FIELD_POINT_INFO(TeraMessageReader reader) : base(reader)
        {
            Points = reader.ReadUInt32();
            MaxPoints = reader.ReadUInt32();
        }
    }
}
