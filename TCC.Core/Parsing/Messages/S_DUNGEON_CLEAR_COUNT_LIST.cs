using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    class S_DUNGEON_CLEAR_COUNT_LIST : ParsedMessage
    {
        public Dictionary<uint, int> DungeonClears { get; set; }
        public S_DUNGEON_CLEAR_COUNT_LIST(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            var playerId = reader.ReadUInt32();
            DungeonClears = new Dictionary<uint, int>();
            for (var i = 0; i < count; i++)
            {
                //reader.BaseStream.Position = 8;
                var current = reader.ReadUInt16();
                var next = reader.ReadUInt16();
                var id = reader.ReadUInt32();
                var clears = reader.ReadInt32();
                reader.Skip(1);
                if(next != 0) reader.BaseStream.Position = next - 4;
                DungeonClears.Add(id, clears);
            }
        }
    }
}
