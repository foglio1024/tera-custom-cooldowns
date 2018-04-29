using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCC.Data;
using TCC.ViewModels;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_GET_USER_LIST : ParsedMessage
    {
        private short count, offset;
        private byte unkh1;
        private int unkh2;//, unkh3;
        private int maxChar;
        private int unkh4;
        private short unkh5;
        private int unkh6;
        private int unkh7;
        private int unkh8;

        public List<Character> CharacterList;
        public List<RawChar> RawCharacters;

        public S_GET_USER_LIST(TeraMessageReader reader) : base(reader)
        {
            CharacterList = new List<Character>();
            RawCharacters = new List<RawChar>();
            count = reader.ReadInt16();
            offset = reader.ReadInt16();
            var nextAddr = offset;
            unkh1 = reader.ReadByte();
            unkh2 = reader.ReadInt32();
            maxChar = reader.ReadInt32();
            unkh4 = reader.ReadInt32();
            unkh5 = reader.ReadInt16();
            unkh6 = reader.ReadInt32();
            unkh7 = reader.ReadInt32();
            unkh8 = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var c = new RawChar();
                reader.BaseStream.Position = nextAddr - 4;
                reader.Skip(2);
                nextAddr = reader.ReadInt16();
                c.unk1 = reader.ReadInt32();
                var nameOffset = reader.ReadInt16();
                c.detailsOffset = reader.ReadInt16();
                c.detailsCount = reader.ReadInt16();
                c.details2offset = reader.ReadInt16();
                c.details2count = reader.ReadInt16();
                c.guildOffset = reader.ReadInt16();
                c.id = reader.ReadUInt32();
                c.gender = reader.ReadInt32();
                c.race = reader.ReadInt32();
                c.charClass = reader.ReadInt32();
                c.level = reader.ReadInt32();
                c.unk2 = reader.ReadInt32();
                c.unk3 = reader.ReadInt32();
                c.loc1 = reader.ReadInt32();
                c.loc2 = reader.ReadInt32();
                c.loc3 = reader.ReadInt32();
                c.lastOnline = reader.ReadInt64();
                c.unk4 = reader.ReadByte();
                c.unk5 = reader.ReadInt32();
                c.unk6 = reader.ReadInt32();
                c.unk7 = reader.ReadInt32();
                c.unk8 = reader.ReadInt32();
                c.earring1 = reader.ReadInt32();
                c.earring2 = reader.ReadInt32();
                c.chest = reader.ReadInt32();
                c.gloves = reader.ReadInt32();
                c.boots = reader.ReadInt32();
                c.unk9 = reader.ReadInt32();
                c.ring1 = reader.ReadInt32();
                c.ring2 = reader.ReadInt32();
                c.innerwear = reader.ReadInt32();
                c.head = reader.ReadInt32();
                c.face = reader.ReadInt32();
                c.appearance = reader.ReadInt64();
                //if(reader.Version < 321150 || reader.Version > 321600)
                reader.Skip(8);
                c.unk10 = reader.ReadInt32();
                c.unk11 = reader.ReadInt32();
                c.unk12 = reader.ReadInt32();
                c.unk13 = reader.ReadInt16();
                c.unk14 = reader.ReadInt32();
                c.unk15 = reader.ReadInt32();
                c.unk16 = reader.ReadInt32();
                c.unk17 = reader.ReadInt32();
                c.unk18 = reader.ReadInt32();
                c.unk19 = reader.ReadInt32();
                c.unk20 = reader.ReadInt32();
                c.unk21 = reader.ReadInt32();
                c.unk22 = reader.ReadInt32();
                c.unk23 = reader.ReadInt32();
                c.unk24 = reader.ReadInt32();
                c.unk25 = reader.ReadInt32();
                c.unk26 = reader.ReadInt32();
                c.unk27 = reader.ReadInt32();
                c.unk28 = reader.ReadInt32();
                c.chestDye = reader.ReadInt32();
                c.glovesDye = reader.ReadInt32();
                c.bootsDye = reader.ReadInt32();
                c.unk29 = reader.ReadInt32();
                c.unk30 = reader.ReadInt32();
                c.unk31 = reader.ReadInt32();
                c.unk32 = reader.ReadInt32();
                c.unk33 = reader.ReadInt32();
                c.unk33b = reader.ReadInt32();
                c.unk33c = reader.ReadInt32();
                c.headDecoration = reader.ReadInt32();
                c.mask = reader.ReadInt32();
                c.backDecoration = reader.ReadInt32();
                c.weaponSkin = reader.ReadInt32();
                c.costume = reader.ReadInt32();
                c.unk35 = reader.ReadInt32();
                c.weaponEnchant = reader.ReadInt32();
                c.curRestExp = reader.ReadInt32();
                c.maxRestExp = reader.ReadInt32();
                try
                {
                    reader.Skip(1); //bool showFace
                    reader.Skip(30 * 4); //floats accTransform
                    reader.Skip(1);
                    reader.Skip(4 + 1); //uint unk, byte unk
                    reader.Skip(1); //bool showStyle
                    c.curRestExpPerc = reader.ReadInt32(); //unk25 from tera-data?
                    c.achiPoints = reader.ReadInt32();
                    c.laurel = reader.ReadInt32();
                    c.pos = reader.ReadInt32();
                    c.guildId = reader.ReadInt32();
                }
                catch (Exception)
                {

                }
                reader.BaseStream.Position = nameOffset - 4;
                c.name = reader.ReadTeraString();

                //c.details = new byte[c.detailsCount];
                //for (int j = 0; j < c.detailsCount; j++)
                //{
                //    c.details[j] = reader.ReadByte();
                //}

                //c.details2 = new byte[c.details2count];
                //for (int k = 0; k < c.details2count; k++)
                //{
                //    c.details2[k] = reader.ReadByte();
                //}

                //c.guild = reader.ReadTeraString();

                CharacterList.Add(new Character(c.name, (Class)c.charClass, c.id, c.pos, InfoWindowViewModel.Instance.GetDispatcher(), (Laurel)c.laurel));
                RawCharacters.Add(c);

            }
            CharacterList = CharacterList.OrderBy(ch => ch.Position).ToList();
        }
    }
}
namespace TCC.Data
{


    public class RawChar
    {
        public int unk1;
        public short nameOffset, detailsOffset, detailsCount, details2offset, details2count, guildOffset;
        public uint id;
        public int gender, race, charClass, level, unk2, unk3;
        public long lastOnline;
        public byte unk4;
        public int unk5, unk6, unk7, unk8;
        public int loc1, loc2, loc3;
        public int earring1, earring2, chest, gloves, boots;
        public int unk9;
        public int ring1, ring2, innerwear, head, face;
        public long appearance;
        public int unk10, unk11, unk12;
        public short unk13;
        public int unk14, unk15, unk16, unk17, unk18, unk19, unk20, unk21, unk22, unk23, unk24, unk25, unk26, unk27, unk28;
        public int chestDye, glovesDye, bootsDye;
        public int unk29, unk30, unk31, unk32, unk33, unk33b, unk33c, headDecoration;
        public int mask;
        public int backDecoration;
        public int weaponSkin, costume;
        public int unk35, curRestExp, weaponEnchant;
        public int curRestExpPerc, maxRestExp;
        public int unk38;
        public short unk39;
        public string name;
        public byte[] details;
        public byte[] details2;
        public string guild;
        public int guildId;
        internal int pos;
        internal int laurel;
        internal byte rested;
        internal int achiPoints;

        public RawChar()
        {

        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Format("Character [{0}] <", pos));
            sb.AppendLine(string.Format("\tName: {0}", name));
            sb.AppendLine(string.Format("\tLevel: {0}", level));
            sb.AppendLine(string.Format("\tClass: {0}", (Class)charClass));
            sb.AppendLine(string.Format("\tLaurel: {0}", (Laurel)laurel));
            sb.AppendLine(">");

            return sb.ToString();
        }
    }
}
