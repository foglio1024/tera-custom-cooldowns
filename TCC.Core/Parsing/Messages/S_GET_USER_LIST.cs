using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCC.Data;
using TCC.Data.Map;
using TCC.Data.Pc;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_GET_USER_LIST : ParsedMessage
    {
        public readonly List<Character> CharacterList;

        public S_GET_USER_LIST(TeraMessageReader reader) : base(reader)
        {
            CharacterList = new List<Character>();
            reader.BaseStream.Position = 0;
            var count = reader.ReadInt16();
            var next = reader.ReadInt16();


            for (var i = 0; i < count; i++)
            {
                var c = new RawChar();
                reader.BaseStream.Position = next - 4;

                reader.Skip(2);
                next = reader.ReadInt16();

                reader.Skip(4);

                var nameOffset = reader.ReadInt16();

                reader.Skip(8); //array offsets and counts

                var guildOffset = reader.ReadInt16();

                c.Id = reader.ReadUInt32();

                reader.Skip(4); //c.gender = reader.ReadInt32();
                reader.Skip(4); //c.race = reader.ReadInt32();
                c.CharClass = reader.ReadInt32();
                c.Level = reader.ReadInt32();
                reader.Skip(8); //c.hp = reader.ReadInt32();
                reader.Skip(4); //c.mp = reader.ReadInt32();
                c.LastLocation = new Location(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
                c.LastOnline = reader.ReadInt64();
                reader.Skip(359);
                c.Laurel = reader.ReadInt32();
                c.Pos = reader.ReadInt32();
                c.GuildId = reader.ReadUInt32();

                reader.BaseStream.Position = nameOffset - 4;
                c.Name = reader.ReadTeraString();
                try
                {
                    reader.BaseStream.Position = guildOffset - 4;
                    c.GuildName = reader.ReadTeraString();
                }
                catch { }

                CharacterList.Add(new Character(c.Name, (Class)c.CharClass, c.Id, c.Pos)
                {
                    GuildName = c.GuildName,
                    Laurel = (Laurel)c.Laurel,
                    Level = c.Level,
                    LastOnline = c.LastOnline,
                    LastLocation = new Location(c.LastLocation.World, c.LastLocation.Guard, c.LastLocation.Section)
                });

            }
            CharacterList = CharacterList.OrderBy(ch => ch.Position).ToList();
        }
    }

    public class RawChar
    {
        public uint Id;
        public uint GuildId;
        public int CharClass, Level;
        public string Name;
        internal int Pos;
        internal int Laurel;
        internal long LastOnline;
        internal Location LastLocation;
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