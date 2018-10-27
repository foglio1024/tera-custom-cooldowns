using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCC.Data;
using TCC.Data.Pc;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;
using TCC.ViewModels;

namespace TCC.Parsing.Messages
{
    public class S_GET_USER_LIST : ParsedMessage
    {
        public readonly List<Character> CharacterList;

        public S_GET_USER_LIST(TeraMessageReader reader) : base(reader)
        {
            CharacterList = new List<Character>();
            var count = reader.ReadInt16();
            var offset = reader.ReadInt16();
            var nextAddr = offset;
            reader.Skip(1); //unkh1 = reader.ReadByte();
            reader.Skip(4); //unkh2 = reader.ReadInt32();
            reader.Skip(4); //maxChar = reader.ReadInt32();
            reader.Skip(4); //unkh4 = reader.ReadInt32();
            reader.Skip(2); //unkh5 = reader.ReadInt16();
            reader.Skip(4); //unkh6 = reader.ReadInt32();
            reader.Skip(4); //unkh7 = reader.ReadInt32();
            reader.Skip(4); //unkh8 = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var c = new RawChar();
                reader.BaseStream.Position = nextAddr - 4;
                reader.Skip(2);
                nextAddr = reader.ReadInt16();
                reader.ReadInt32();
                var nameOffset = reader.ReadInt16();
                reader.Skip(2); //c.detailsOffset = reader.ReadInt16();
                reader.Skip(2); //c.detailsCount = reader.ReadInt16();
                reader.Skip(2); //c.details2offset = reader.ReadInt16();
                reader.Skip(2); //c.details2count = reader.ReadInt16();
                var guildOffset = reader.ReadInt16();
                c.Id = reader.ReadUInt32();
                reader.Skip(4); //c.gender = reader.ReadInt32();
                reader.Skip(4); //c.race = reader.ReadInt32();
                c.CharClass = reader.ReadInt32();
                c.Level = reader.ReadInt32();
                reader.Skip(4); //c.unk2 = reader.ReadInt32();
                reader.Skip(4); //c.unk3 = reader.ReadInt32();
                reader.Skip(4); //c.loc1 = reader.ReadInt32();
                reader.Skip(4); //c.loc2 = reader.ReadInt32();
                reader.Skip(4); //c.loc3 = reader.ReadInt32();
                reader.Skip(8); //c.lastOnline = reader.ReadInt64();
                reader.Skip(1); //c.unk4 = reader.ReadByte();
                reader.Skip(4); //c.unk5 = reader.ReadInt32();
                reader.Skip(4); //c.unk6 = reader.ReadInt32();
                reader.Skip(4); //c.unk7 = reader.ReadInt32();
                reader.Skip(4); //c.unk8 = reader.ReadInt32();
                reader.Skip(4); //c.earring1 = reader.ReadInt32();
                reader.Skip(4); //c.earring2 = reader.ReadInt32();
                reader.Skip(4); //c.chest = reader.ReadInt32();
                reader.Skip(4); //c.gloves = reader.ReadInt32();
                reader.Skip(4); //c.boots = reader.ReadInt32();
                reader.Skip(4); //c.unk9 = reader.ReadInt32();
                reader.Skip(4); //c.ring1 = reader.ReadInt32();
                reader.Skip(4); //c.ring2 = reader.ReadInt32();
                reader.Skip(4); //c.innerwear = reader.ReadInt32();
                reader.Skip(4); //c.head = reader.ReadInt32();
                reader.Skip(4); //c.face = reader.ReadInt32();
                reader.Skip(8); //c.appearance = reader.ReadInt64();
                //if(reader.Version < 321150 || reader.Version > 321600)
                reader.Skip(8);
                reader.Skip(4); //c.unk10 = reader.ReadInt32();
                reader.Skip(4); //c.unk11 = reader.ReadInt32();
                reader.Skip(4); //c.unk12 = reader.ReadInt32();
                reader.Skip(2); //c.unk13 = reader.ReadInt16();
                reader.Skip(4); //c.unk14 = reader.ReadInt32();
                reader.Skip(4); //c.unk15 = reader.ReadInt32();
                reader.Skip(4); //c.unk16 = reader.ReadInt32();
                reader.Skip(4); //c.unk17 = reader.ReadInt32();
                reader.Skip(4); //c.unk18 = reader.ReadInt32();
                reader.Skip(4); //c.unk19 = reader.ReadInt32();
                reader.Skip(4); //c.unk20 = reader.ReadInt32();
                reader.Skip(4); //c.unk21 = reader.ReadInt32();
                reader.Skip(4); //c.unk22 = reader.ReadInt32();
                reader.Skip(4); //c.unk23 = reader.ReadInt32();
                reader.Skip(4); //c.unk24 = reader.ReadInt32();
                reader.Skip(4); //c.unk25 = reader.ReadInt32();
                reader.Skip(4); //c.unk26 = reader.ReadInt32();
                reader.Skip(4); //c.unk27 = reader.ReadInt32();
                reader.Skip(4); //c.unk28 = reader.ReadInt32();
                reader.Skip(4); //c.chestDye = reader.ReadInt32();
                reader.Skip(4); //c.glovesDye = reader.ReadInt32();
                reader.Skip(4); //c.bootsDye = reader.ReadInt32();
                reader.Skip(4); //c.unk29 = reader.ReadInt32();
                reader.Skip(4); //c.unk30 = reader.ReadInt32();
                reader.Skip(4); //c.unk31 = reader.ReadInt32();
                reader.Skip(4); //c.unk32 = reader.ReadInt32();
                reader.Skip(4); //c.unk33 = reader.ReadInt32();
                reader.Skip(4); //c.unk33b = reader.ReadInt32();
                reader.Skip(4); //c.unk33c = reader.ReadInt32();
                reader.Skip(4); //c.headDecoration = reader.ReadInt32();
                reader.Skip(4); //c.mask = reader.ReadInt32();
                reader.Skip(4); //c.backDecoration = reader.ReadInt32();
                reader.Skip(4); //c.weaponSkin = reader.ReadInt32();
                reader.Skip(4); //c.costume = reader.ReadInt32();
                reader.Skip(4); //c.unk35 = reader.ReadInt32();
                reader.Skip(4); //c.weaponEnchant = reader.ReadInt32();
                reader.Skip(4); //c.curRestExp = reader.ReadInt32();
                reader.Skip(4); //c.maxRestExp = reader.ReadInt32();
                try
                {
                    reader.Skip(1); //bool showFace
                    reader.Skip(30 * 4); //floats accTransform
                    reader.Skip(1);
                    reader.Skip(4 + 1); //uint unk, byte unk
                    reader.Skip(1); //bool showStyle
                    reader.Skip(4); //c.curRestExpPerc = reader.ReadInt32(); //unk25 from tera-data?
                    reader.Skip(4); //c.achiPoints = reader.ReadInt32();
                    c.Laurel = reader.ReadInt32();
                    c.Pos = reader.ReadInt32();
                    reader.Skip(4); //c.guildId = reader.ReadInt32();
                }
                catch
                {
                    // ignored
                }

                reader.BaseStream.Position = nameOffset - 4;
                c.Name = reader.ReadTeraString();
                try
                {
                    reader.BaseStream.Position = guildOffset - 4;
                    c.GuildName = reader.ReadTeraString();
                }
                catch (Exception) { }

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

                CharacterList.Add(new Character(c.Name, (Class)c.CharClass, c.Id, c.Pos, InfoWindowViewModel.Instance.GetDispatcher(), (Laurel)c.Laurel) { GuildName = c.GuildName });

            }
            CharacterList = CharacterList.OrderBy(ch => ch.Position).ToList();
        }
    }

    public class RawChar
    {
        public uint Id;
        public int CharClass, Level;
        public string Name;
        internal int Pos;
        internal int Laurel;
        public string GuildName = "";

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Character [{Pos}] <");
            sb.AppendLine($"\tName: {Name}");
            sb.AppendLine($"\tLevel: {Level}");
            sb.AppendLine($"\tClass: {(Class)CharClass}");
            sb.AppendLine($"\tLaurel: {(Laurel)Laurel}");
            sb.AppendLine(">");

            return sb.ToString();
        }
    }
}