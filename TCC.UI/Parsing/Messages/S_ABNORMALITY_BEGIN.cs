using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    public class S_ABNORMALITY_BEGIN : ParsedMessage
    {

        public ulong targetId;
        public ulong casterId;
        public uint id;
        public int duration;
        public int stacks;

        public S_ABNORMALITY_BEGIN(TeraMessageReader reader) : base(reader)
        {
            targetId = reader.ReadUInt64();
            casterId = reader.ReadUInt64();
            id = reader.ReadUInt32();
            duration = reader.ReadInt32();
            reader.Skip(4);
            stacks = reader.ReadInt32();

            //Console.WriteLine("[S_ABNORMALITY_BEGIN] target:{0} id:{1} duration:{2} stacks:{3}", targetId, id, duration, stacks);
        }
    }
}
