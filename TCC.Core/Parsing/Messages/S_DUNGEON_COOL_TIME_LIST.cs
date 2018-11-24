using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_DUNGEON_COOL_TIME_LIST : ParsedMessage
    {
        public readonly Dictionary<uint, short> DungeonCooldowns;
        public S_DUNGEON_COOL_TIME_LIST(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            reader.Skip(2); //var offset = reader.ReadUInt16();
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
