using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_NPC : ParsedMessage
    {

        ulong id;
        uint npc;
        short type;

        public ulong EntityId { get => id;}
        public uint Npc { get => npc;  }
        public short Type { get => type; }

        public S_SPAWN_NPC(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(10);
            id = reader.ReadUInt64();
            reader.Skip(26);
            npc = reader.ReadUInt32();
            type = reader.ReadInt16();

           // Console.WriteLine("[S_SPAWN NPC] id:{0} name:{1}", id, MonsterDatabase.GetName(npc, (uint)type));
        }
    }
}
