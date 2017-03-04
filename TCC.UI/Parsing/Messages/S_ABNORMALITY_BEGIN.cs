using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.UI.Messages
{
    class S_ABNORMALITY_BEGIN : ParsedMessage
    {
        public bool isHurricane;
        public ulong casterId;
        
        public S_ABNORMALITY_BEGIN(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(8);
            casterId = reader.ReadUInt64();
            if (reader.ReadUInt32() == 60010)
            {
                isHurricane = true;
            }
            else
            {
                isHurricane = false;
            }

        }
    }
}
