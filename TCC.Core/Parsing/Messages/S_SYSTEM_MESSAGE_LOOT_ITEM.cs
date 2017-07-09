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
    public class S_SYSTEM_MESSAGE_LOOT_ITEM : ParsedMessage
    {
        public int ItemId { get; private set; }
        public int Amount { get; set; }
        public string SysMessage { get; private set; }

        private int unk1, unk2, unk3, unk4;
        private byte unk5, unk6, unk7;

        public S_SYSTEM_MESSAGE_LOOT_ITEM(TeraMessageReader reader) : base(reader)
        {
            var msgOffset = reader.ReadUInt16();

            ItemId = reader.ReadInt32();
            unk1 = reader.ReadInt32();
            Amount = reader.ReadInt32();
            unk2 = reader.ReadInt32();
            unk3 = reader.ReadInt32();
            unk4 = reader.ReadInt32();
            unk5 = reader.ReadByte();
            unk6 = reader.ReadByte();
            unk7 = reader.ReadByte();

            SysMessage = reader.ReadTeraString();

            Debug.WriteLine(nameof(S_SYSTEM_MESSAGE_LOOT_ITEM) + " " + SysMessage);
            Debug.WriteLine("\t {0} x{1}", ItemId ,Amount);
            Debug.WriteLine("\t u1:{0}", unk1);
            Debug.WriteLine("\t u2:{0}", unk2);
            Debug.WriteLine("\t u3:{0}", unk3);
            Debug.WriteLine("\t u4:{0}", unk4);
            Debug.WriteLine("\t u5:{0}", unk5);
            Debug.WriteLine("\t u6:{0}", unk6);
            Debug.WriteLine("\t u7:{0}", unk7);
        }
    }
}
