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
        int currentHP, maxHP, diff;
        uint type;
        ulong target, source;
        byte crit;

        public int CurrentHP { get => currentHP; }
        public int MaxHP { get => maxHP; }
        public int Diff { get => diff; }
        public ulong Target { get => target; }
        public ulong Source { get => source; }
        public byte Crit { get => crit; }

        public S_CREATURE_CHANGE_HP(TeraMessageReader reader) : base(reader)
        {
            currentHP = reader.ReadInt32();
            maxHP = reader.ReadInt32();
            diff = reader.ReadInt32();
            type = reader.ReadUInt32();
            target = reader.ReadUInt64();
            source = reader.ReadUInt64();
            crit = reader.ReadByte();
        }
    }
}
