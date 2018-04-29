using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class C_SHOW_ITEM_TOOLTIP_EX : ParsedMessage
    {
        private ushort nameOffset;
        private int unk1;
        private ulong uid;
        private int unk2;
        private int unk3;
        private int unk4;
        private int unk5;
        private int unk6;
        private string name;
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

            //Debug.WriteLine("uid:{0} unk1:{1} unk2:{2} unk3:{3} unk4:{4} unk5:{5} unk6:{6} name:{7}", uid, unk1, unk2, unk3, unk4, unk5, unk6, name);
        }
    }
}
