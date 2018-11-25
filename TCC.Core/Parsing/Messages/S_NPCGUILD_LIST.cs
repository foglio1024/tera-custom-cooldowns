using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_NPCGUILD_LIST : ParsedMessage
    {
        public Dictionary<int, int> NpcGuildList { get; }
        public ulong UserId { get; }
        public S_NPCGUILD_LIST(TeraMessageReader reader) : base(reader)
        {
            NpcGuildList = new Dictionary<int, int>();
            var count = reader.ReadUInt16();
            if (count == 0) return;
            var offset = reader.ReadUInt16();
            UserId = reader.ReadUInt64();
            reader.BaseStream.Position = offset - 4;
            for (var i = 0; i < count; i++)
            {
                var curr = reader.ReadUInt16();
                var next = reader.ReadUInt16();

                var region = reader.ReadInt32();
                var faction = reader.ReadInt32();
                var rank = reader.ReadInt32();
                var reputation = reader.ReadInt32();
                var credits = reader.ReadInt32();
                NpcGuildList[faction] = credits;
                if (next == 0) return;
                reader.BaseStream.Position = next - 4;
            }
        }
    }
}