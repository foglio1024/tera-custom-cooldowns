using System.Collections.Generic;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    class S_DUNGEON_COOL_TIME_LIST : ParsedMessage
    {
        public Dictionary<uint, short> DungeonCooldowns;
        public S_DUNGEON_COOL_TIME_LIST(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            reader.Skip(4);
            DungeonCooldowns = new Dictionary<uint, short>();
            for (var i = 0; i < count; i++)
            {
                reader.Skip(4);
                var id = reader.ReadUInt32();
                reader.Skip(8);
                var entries = reader.ReadInt16();
                DungeonCooldowns.Add(id, entries);
            }
        }
    }
}
