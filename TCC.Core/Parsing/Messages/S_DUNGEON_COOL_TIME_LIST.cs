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
            DungeonCooldowns = new Dictionary<uint, short>();

            try
            {
                reader.RepositionAt(4);
                var count = reader.ReadUInt16();
                reader.Skip(2); //var offset = reader.ReadUInt16();
                reader.Skip(4);
                for (var i = 0; i < count; i++)
                {
                    reader.Skip(2); //var curr = reader.ReadUInt16();
                    var next = reader.ReadUInt16();
                    var id = reader.ReadUInt32();
                    reader.Skip(8);
                    var daily = reader.ReadInt16();
                    var weekly = reader.ReadInt16();
                    var entries = (short) (daily == -1 ? weekly == -1 ? -1 : weekly : daily);
                    if(entries != -1) DungeonCooldowns[id] = entries;
                    if (next == 0) return;
                    reader.RepositionAt(next);
                }
            }
            catch (System.Exception)
            {
                Log.F($"[S_DUNGEON_COOL_TIME_LIST] Failed to parse packet");
            }
        }
    }
}
