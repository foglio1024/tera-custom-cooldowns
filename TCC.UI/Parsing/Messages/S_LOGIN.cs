using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    class S_LOGIN : ParsedMessage
    {
        short nameOffset { get; set; }
        short detailsOffset { get; set; }
        short detailsCount { get; set; }
        short details2Offset { get; set; }
        short details2Count { get; set; }
        public ulong entityId { get; set; }
        ulong pId { get; set; }
        int unk1 { get; set; } 
        int unk3 { get; set; } 
        int unk4 { get; set; } 
        int unk5 { get; set; }
        byte unk2{ get; set; }
        ulong appearance { get; set; }
        short unk6 { get; set; }
        short level       { get; set; }
        short gatherEn    { get; set; }
        short gatherUnk   { get; set; }
        short gatherPl    { get; set; }
        short gatherMin   { get; set; }
        int unk7 { get; set; }
        int unk8{ get; set; }
        short unk9 { get; set; }
        uint model { get; set; }
        long expTot { get; set; }
        long expShown { get; set; }
        long expNeeded{ get; set; }
        int unk10 { get; set; }
        int unk10b { get; set; }
        int unk10c { get; set; }
        int unk10d { get; set; }
        int restCurr { get; set; }
        int restMax { get; set; }
        float unk11 { get; set; }
        int unk12{ get; set; } 
        int weap{ get; set; } 
        int chest{ get; set; } 
        int gloves { get; set; }
        int boots{ get; set; } 
        int innerWear{ get; set; } 
        int head{ get; set; } 
        int face{ get; set; } 
        int unk13{ get; set; } 
        int unk14{ get; set; } 
        int unk16{ get; set; } 
        int unk17{ get; set; } 
        int title{ get; set; } 
        int weapMod{ get; set; } 
        int chestMod{ get; set; }
        int glovesMod{ get; set; }
        int bootsMod{ get; set; } 
        int unk19{ get; set; } 
        int unk20{ get; set; } 
        int unk21{ get; set; } 
        int unk22{ get; set; } 
        int unk23{ get; set; } 
        int unk24{ get; set; } 
        int unk25{ get; set; } 
        int unk26{ get; set; } 
        int weapEnch{ get; set; } 
        int unk27 { get; set; }
        byte unk15{ get; set; } 
        byte unk28{ get; set; } 
        byte unk29 { get; set; }
        int hairAdorn{ get; set; } 
        int mask{ get; set; } 
        int back{ get; set; } 
        int weapSkin{ get; set; } 
        int costume{ get; set; } 
        int unk30{ get; set; } 
        int unk31{ get; set; } 
        int unk32{ get; set; } 
        int unk33{ get; set; } 
        int unk35{ get; set; } 
        int unk36{ get; set; } 
        short unk37{ get; set; }
        float unk38{ get; set; }
        int unk39{ get; set; }
        byte unk40{ get; set; }
        int unk41{ get; set; }
        public string Name{ get; set; }
        byte[] details{ get; set; }
        byte[] details2{ get; set; }
        byte unk34{ get; set; }
        
        public Class CharacterClass { get
            {
                int classId = (int)(model - 10101) % 100;
                return (Class)classId;
            }
        }

        public S_LOGIN(TeraMessageReader reader) : base(reader)
        {
            nameOffset = reader.ReadInt16();
            detailsOffset = reader.ReadInt16();
            detailsCount = reader.ReadInt16();
            details2Offset = reader.ReadInt16();
            details2Count = reader.ReadInt16();
            model = reader.ReadUInt32();
            entityId = reader.ReadUInt64();
            pId = reader.ReadUInt64();
            unk1 = reader.ReadInt32();
            unk2=reader.ReadByte();
            unk3=reader.ReadInt32();
            unk4=reader.ReadInt32();
            unk5=reader.ReadInt32();
            appearance = reader.ReadUInt64();
            unk6=reader.ReadInt16();
            level=reader.ReadInt16();
            gatherEn=reader.ReadInt16();
            gatherUnk=reader.ReadInt16();
            gatherPl=reader.ReadInt16();
            gatherMin=reader.ReadInt16();
            unk7=reader.ReadInt32();
            unk8=reader.ReadInt32();
            unk9=reader.ReadInt16();
            expTot=reader.ReadInt64();
            expShown=reader.ReadInt64();
            expNeeded=reader.ReadInt64();
            unk10=reader.ReadInt32(); 
            unk10b=reader.ReadInt32();
            unk10c=reader.ReadInt32();
            unk10d=reader.ReadInt32();
            restCurr = reader.ReadInt32();
            restMax  = reader.ReadInt32();
            unk11=reader.ReadSingle();
            unk12=reader.ReadInt32();
            weap=reader.ReadInt32();
            chest =reader.ReadInt32();
            gloves=reader.ReadInt32();
            boots =reader.ReadInt32();
            innerWear=reader.ReadInt32();
            head = reader.ReadInt32();
            face= reader.ReadInt32();
            unk13=reader.ReadInt32();
            unk14=reader.ReadInt32();
            unk15=reader.ReadByte(); 
            unk16=reader.ReadInt32();
            unk17=reader.ReadInt32();
            title=reader.ReadInt32();
            weapMod=reader.ReadInt32();
            chestMod=reader.ReadInt32();
            glovesMod=reader.ReadInt32();
            bootsMod=reader.ReadInt32();
            unk19=reader.ReadInt32();
            unk20=reader.ReadInt32();
            unk21=reader.ReadInt32();
            unk22=reader.ReadInt32();
            unk23=reader.ReadInt32();
            unk24=reader.ReadInt32();
            unk25=reader.ReadInt32();
            unk26=reader.ReadInt32();
            weapEnch= reader.ReadInt32();
            unk27= reader.ReadInt32();
            unk28= reader.ReadByte(); 
            unk29= reader.ReadByte(); 
            hairAdorn= reader.ReadInt32();
            mask=reader.ReadInt32();
            back=reader.ReadInt32();
            weapSkin=reader.ReadInt32();
            costume = reader.ReadInt32();
            unk30=reader.ReadInt32(); 
            unk31=reader.ReadInt32(); 
            unk32=reader.ReadInt32(); 
            unk33=reader.ReadInt32(); 
            unk35=reader.ReadInt32(); 
            unk36=reader.ReadInt32();
            unk37 = reader.ReadInt16();
            Name= reader.ReadTeraString();
            details = new byte[detailsCount];
            details2 = new byte[details2Count];
            for (int i = 0; i < detailsCount; i++)
            {
                details[i] = reader.ReadByte();
            }
            for (int i = 0; i < details2Count; i++)
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
