using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.UI.Messages
{
    class S_LOGIN : ParsedMessage
    {
        public ulong entityId;
        uint model;

        public Class GetClass()
        {
            int classId = (int)(model - 10101) % 100;

            return (Class)classId;
        }

        public S_LOGIN(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(10);
            model = reader.ReadUInt32();
            entityId = reader.ReadUInt64();
        }
    }
}
