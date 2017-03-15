using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC
{
    public class S_CREATURE_CHANGE_HP : ParsedMessage
    {
        public uint currentHP, maxHP, diff;
        public uint type;
        public ulong target, source;
        public byte crit;
        public S_CREATURE_CHANGE_HP(TeraMessageReader reader) : base(reader)
        {
            currentHP = reader.ReadUInt32();
            maxHP = reader.ReadUInt32();
            diff = reader.ReadUInt32();
            type = reader.ReadUInt32();
            target = reader.ReadUInt64();
            source = reader.ReadUInt64();
            crit = reader.ReadByte();
        }
    }
}
