using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_DUNGEON_EVENT_MESSAGE : ParsedMessage
    {
        public uint MessageId { get; private set; }
        public S_DUNGEON_EVENT_MESSAGE(TeraMessageReader reader) : base(reader)
        {
            var o = reader.ReadUInt16();
            reader.BaseStream.Position = o - 4;
            MessageId = UInt32.Parse(reader.ReadTeraString().Substring("@dungeon:".Length));
        }
    }
}
