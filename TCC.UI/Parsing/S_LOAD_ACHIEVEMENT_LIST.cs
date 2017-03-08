using Tera.Game;
using Tera.Game.Messages;

namespace TCC
{
    internal class S_LOAD_ACHIEVEMENT_LIST : ParsedMessage
    {
        const int ACHIEVEMENT_LENGHT = 12;
        short achiCount, achiOffset, historyCount, historyOffset;
        long cid;
        int unk1, achis, unk3, unk4;

        int laurel;
        public Laurel Laurel
        {
            get
            {
                return (Laurel)laurel;
            }
        }

        public S_LOAD_ACHIEVEMENT_LIST(TeraMessageReader reader) : base(reader)
        {
            achiCount = reader.ReadInt16();
            achiOffset = reader.ReadInt16();
            historyCount = reader.ReadInt16();
            historyOffset = reader.ReadInt16();
            cid = reader.ReadInt64();
            unk1 = reader.ReadInt32();
            achis = reader.ReadInt32();
            unk3 = reader.ReadInt32();
            unk4 = reader.ReadInt32();
            reader.Skip(ACHIEVEMENT_LENGHT * achiCount);
            //System.Console.WriteLine("{0}-{1}-{2}", unk1, unk3, unk4);


        }
    }
}