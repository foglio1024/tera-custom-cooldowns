using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_DUNGEON_CLEAR_COUNT_LIST : ParsedMessage
    {
        public bool Failed { get; }
        public Dictionary<uint, int> DungeonClears { get; }
        public uint PlayerId { get; }
        public S_DUNGEON_CLEAR_COUNT_LIST(TeraMessageReader reader) : base(reader)
        {
            try
            {
                var count = reader.ReadUInt16();
                reader.Skip(2); //var offset = reader.ReadUInt16();
                PlayerId = reader.ReadUInt32();
                DungeonClears = new Dictionary<uint, int>();
                for (var i = 0; i < count; i++)
                {
                    //reader.BaseStream.Position = 8;
                    reader.Skip(2); //var current = reader.ReadUInt16();
                    var next = reader.ReadUInt16();
                    var id = reader.ReadUInt32();
                    var clears = reader.ReadInt32();
                    reader.Skip(1);
                    if (next != 0) reader.BaseStream.Position = next - 4;
                    DungeonClears.Add(id, clears);
                }
            }
            catch
            {
                Failed = true;
            }
        }
    }
}
