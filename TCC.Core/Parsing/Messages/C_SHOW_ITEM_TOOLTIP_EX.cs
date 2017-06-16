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
    class C_SHOW_ITEM_TOOLTIP_EX : ParsedMessage
    {
        ushort nameOffset;
        int unk1;
        ulong uid;
        int unk2;
        int unk3;
        int unk4;
        int unk5;
        int unk6;
        string name;
        public C_SHOW_ITEM_TOOLTIP_EX(TeraMessageReader reader) : base(reader)
        {
            nameOffset = reader.ReadUInt16();
            unk1 = reader.ReadInt32();
            uid = reader.ReadUInt64();
            unk2 = reader.ReadInt32();
            unk3 = reader.ReadInt32();
            unk4 = reader.ReadInt32();
            unk5 = reader.ReadInt32();
            unk6 = reader.ReadInt32();
            name = reader.ReadTeraString();

            Debug.WriteLine("uid:{0} unk1:{1} unk2:{2} unk3:{3} unk4:{4} unk5:{5} unk6:{6} name:{7}", uid, unk1, unk2, unk3, unk4, unk5, unk6, name);
        }
    }
}
