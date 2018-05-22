using TCC.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_LOGIN : ParsedMessage
    {
        private short nameOffset { get; set; }
        private short detailsOffset { get; set; }
        private short detailsCount { get; set; }
        private short details2Offset { get; set; }
        private short details2Count { get; set; }
        public ulong entityId { get; set; }
        public uint ServerId { get; set; }
        public uint PlayerId { get; set; }
        private int unk1 { get; set; }
        private int unk3 { get; set; }
        private int unk4 { get; set; }
        private int unk5 { get; set; }
        private byte unk2 { get; set; }
        private ulong appearance { get; set; }
        private short unk6 { get; set; }
        public short Level { get; set; }
        private short gatherEn { get; set; }
        private short gatherUnk { get; set; }
        private short gatherPl { get; set; }
        private short gatherMin { get; set; }
        private int unk7 { get; set; }
        private int unk8 { get; set; }
        private short unk9 { get; set; }
        private uint model { get; set; }
        private long expTot { get; set; }
        private long expShown { get; set; }
        private long expNeeded { get; set; }
        private int unk10 { get; set; }
        private int unk10b { get; set; }
        private int unk10c { get; set; }
        private int unk10d { get; set; }
        private int restCurr { get; set; }
        private int restMax { get; set; }
        private float unk11 { get; set; }
        private int unk12 { get; set; }
        private int weap { get; set; }
        private int chest { get; set; }
        private int gloves { get; set; }
        private int boots { get; set; }
        private int innerWear { get; set; }
        private int head { get; set; }
        private int face { get; set; }
        private int unk13 { get; set; }
        private int unk14 { get; set; }
        private int unk16 { get; set; }
        private int unk17 { get; set; }
        private int title { get; set; }
        private int weapMod { get; set; }
        private int chestMod { get; set; }
        private int glovesMod { get; set; }
        private int bootsMod { get; set; }
        private int unk19 { get; set; }
        private int unk20 { get; set; }
        private int unk21 { get; set; }
        private int unk22 { get; set; }
        private int unk23 { get; set; }
        private int unk24 { get; set; }
        private int unk25 { get; set; }
        private int unk26 { get; set; }
        private int weapEnch { get; set; }
        private int unk27 { get; set; }
        private byte unk15 { get; set; }
        private byte unk28 { get; set; }
        private byte unk29 { get; set; }
        private int hairAdorn { get; set; }
        private int mask { get; set; }
        private int back { get; set; }
        private int weapSkin { get; set; }
        private int costume { get; set; }
        private int unk30 { get; set; }
        private int unk31 { get; set; }
        private int unk32 { get; set; }
        private int unk33 { get; set; }
        private int unk35 { get; set; }
        private int unk36 { get; set; }
        private short unk37 { get; set; }
        private float unk38 { get; set; }
        private int unk39 { get; set; }
        private byte unk40 { get; set; }
        private int unk41 { get; set; }
        public string Name { get; set; }
        private byte[] details { get; set; }
        private byte[] details2 { get; set; }
        private byte unk34 { get; set; }


        public Class CharacterClass
        {
            get
            {
                var classId = (int)(model - 10101) % 100;
                return (Class)classId;
            }
        }

        public S_LOGIN(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            nameOffset = reader.ReadInt16();
            //detailsOffset = reader.ReadInt16();
            //detailsCount = reader.ReadInt16();
            //details2Offset = reader.ReadInt16();
            //details2Count = reader.ReadInt16();
            reader.Skip(8);
            model = reader.ReadUInt32();
            entityId = reader.ReadUInt64();
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            //unk1 = reader.ReadInt32();
            //unk2=reader.ReadByte();
            //unk3=reader.ReadInt32();
            //unk4=reader.ReadInt32();
            //unk5=reader.ReadInt32();
            reader.Skip(17);
            appearance = reader.ReadUInt64();
            //unk6=reader.ReadInt16();
            reader.Skip(2);
            Level = reader.ReadInt16();
            //gatherEn=reader.ReadInt16();
            //gatherUnk=reader.ReadInt16();
            //gatherPl=reader.ReadInt16();
            //gatherMin=reader.ReadInt16();
            //unk7=reader.ReadInt32();
            //unk8=reader.ReadInt32();
            //unk9=reader.ReadInt16();
            //expTot=reader.ReadInt64();
            //expShown=reader.ReadInt64();
            //expNeeded=reader.ReadInt64();
            //unk10=reader.ReadInt32(); 
            //unk10b=reader.ReadInt32();
            //unk10c=reader.ReadInt32();
            //unk10d=reader.ReadInt32();
            reader.Skip(58);
            restCurr = reader.ReadInt32();
            restMax = reader.ReadInt32();
            //unk11=reader.ReadSingle();
            //unk12=reader.ReadInt32();
            reader.Skip(8);
            weap = reader.ReadInt32();
            chest = reader.ReadInt32();
            gloves = reader.ReadInt32();
            boots = reader.ReadInt32();
            innerWear = reader.ReadInt32();
            head = reader.ReadInt32();
            face = reader.ReadInt32();
            //unk13=reader.ReadInt32();
            //unk14=reader.ReadInt32();
            //unk15=reader.ReadByte(); 
            //unk16=reader.ReadInt32();
            //unk17=reader.ReadInt32();
            reader.Skip(17);
            title = reader.ReadInt32();
            weapMod = reader.ReadInt32();
            chestMod = reader.ReadInt32();
            glovesMod = reader.ReadInt32();
            bootsMod = reader.ReadInt32();
            //unk19=reader.ReadInt32();
            //unk20=reader.ReadInt32();
            //unk21=reader.ReadInt32();
            //unk22=reader.ReadInt32();
            //unk23=reader.ReadInt32();
            //unk24=reader.ReadInt32();
            //unk25=reader.ReadInt32();
            //unk26=reader.ReadInt32();
            reader.Skip(32);
            weapEnch = reader.ReadInt32();
            //unk27= reader.ReadInt32();
            //unk28= reader.ReadByte(); 
            //unk29= reader.ReadByte(); 
            reader.Skip(6);
            hairAdorn = reader.ReadInt32();
            mask = reader.ReadInt32();
            back = reader.ReadInt32();
            weapSkin = reader.ReadInt32();
            costume = reader.ReadInt32();
            //unk30=reader.ReadInt32(); 
            //unk31=reader.ReadInt32(); 
            //unk32=reader.ReadInt32(); 
            //unk33=reader.ReadInt32(); 
            //unk35=reader.ReadInt32(); 
            //unk36=reader.ReadInt32();
            //unk37 = reader.ReadInt16();
            reader.BaseStream.Position = nameOffset - 4;
            Name = reader.ReadTeraString();
            return;
            details = new byte[detailsCount];
            details2 = new byte[details2Count];
            for (var i = 0; i < detailsCount; i++)
            {
                details[i] = reader.ReadByte();
            }
            for (var i = 0; i < details2Count; i++)
            {
                details2[i] = reader.ReadByte();
            }
            //Console.WriteLine();
            //Console.Write("unk1: {0} - ", unk1);
            //Console.Write("unk2: {0} - ", unk2);
            //Console.Write("unk3: {0} - ", unk3);
            //Console.Write("unk4: {0} - ", unk4);
            //Console.Write("unk5: {0} - ", unk5);
            //Console.Write("unk6: {0} - ", unk6);
            //Console.Write("unk7: {0} - ", unk7);
            //Console.Write("unk8: {0} - ", unk8);
            //Console.Write("unk9: {0} - ", unk9);
            //Console.Write("unk10: {0} - ", unk10);
            //Console.Write("unk11: {0} - ", unk11);
            //Console.Write("unk12: {0} - ", unk12);
            //Console.Write("unk13: {0} - ", unk13);
            //Console.Write("unk14: {0} - ", unk14);
            //Console.Write("unk15: {0} - ", unk15);
            //Console.Write("unk16: {0} - ", unk16);
            //Console.Write("unk17: {0} - ", unk17);
            //Console.Write("unk19: {0} - ", unk19);
            //Console.Write("unk20: {0} - ", unk20);
            //Console.Write("unk21: {0} - ", unk21);
            //Console.Write("unk22: {0} - ", unk22);
            //Console.Write("unk23: {0} - ", unk23);
            //Console.Write("unk24: {0} - ", unk24);
            //Console.Write("unk25: {0} - ", unk25);
            //Console.Write("unk26: {0} - ", unk26);
            //Console.Write("unk27: {0} - ", unk27);
            //Console.Write("unk28: {0} - ", unk28);
            //Console.Write("unk29: {0} - ", unk29);
            //Console.Write("unk30: {0} - ", unk30);
            //Console.Write("unk31: {0} - ", unk31);
            //Console.Write("unk32: {0} - ", unk32);
            //Console.Write("unk33: {0} - ", unk33);
            //Console.Write("unk34: {0} - ", unk34);
            //Console.Write("unk35: {0} - ", unk35);
            //Console.Write("unk36: {0} - ", unk36);
            //Console.Write("unk37: {0} - ", unk37);

        }
    }
}
