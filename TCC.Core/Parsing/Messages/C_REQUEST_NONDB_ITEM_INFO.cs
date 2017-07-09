using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class C_REQUEST_NONDB_ITEM_INFO : ParsedMessage
    {
        int item, unk1, unk2;
        public C_REQUEST_NONDB_ITEM_INFO(TeraMessageReader reader) : base(reader)
        {
            item = reader.ReadInt32();
            unk1 = reader.ReadInt32();
            unk2 = reader.ReadInt32();
            //Debug.WriteLine("item:{0} unk1:{1} unk2:{2}", item, unk1, unk2);
        }
    }
}
