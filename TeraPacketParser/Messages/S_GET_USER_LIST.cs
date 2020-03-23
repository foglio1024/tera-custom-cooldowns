using System.Collections.Generic;
using System.Linq;

using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_GET_USER_LIST : ParsedMessage
    {
        public readonly List<CharacterData> CharacterList;

        public S_GET_USER_LIST(TeraMessageReader reader) : base(reader)
        {
            CharacterList = new List<CharacterData>();
            var count = reader.ReadInt16();
            var next = reader.ReadInt16();


            for (var i = 0; i < count; i++)
            {
                var c = new CharacterData();
                reader.RepositionAt(next);

                reader.Skip(2);
                next = reader.ReadInt16();

                reader.Skip(4);

                var nameOffset = reader.ReadInt16();

                reader.Skip(8); //array offsets and counts

                var guildOffset = reader.ReadInt16();

                c.Id = reader.ReadUInt32();

                reader.Skip(4); //c.gender = reader.ReadInt32();
                reader.Skip(4); //c.race = reader.ReadInt32();
                c.CharClass = (Class)reader.ReadInt32();
                c.Level = reader.ReadInt32();
                reader.Skip(8); //c.hp = reader.ReadInt32();
                reader.Skip(4); //c.mp = reader.ReadInt32();
                //c.LastLocation = new Location(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
                c.LastWorldId = reader.ReadUInt32();
                c.LastGuardId = reader.ReadUInt32();
                c.LastSectionId = reader.ReadUInt32();
                c.LastOnline = reader.ReadInt64();
                reader.Skip(reader.Factory.ReleaseVersion >= 8600 && reader.Factory.ReleaseVersion < 9901 ? 367 : 359);
                c.Laurel = (Laurel)reader.ReadInt32();
                c.Position = reader.ReadInt32();
                c.GuildId = reader.ReadUInt32();

                reader.RepositionAt(nameOffset);
                c.Name = reader.ReadTeraString();
                try
                {
                    reader.RepositionAt(guildOffset);
                    c.GuildName = reader.ReadTeraString();
                }
                catch { }
                CharacterList.Add(c);
                //CharacterList.Add(new Character(c.Name, (Class)c.CharClass, c.Id, c.Pos)
                //{
                //    GuildName = c.GuildName,
                //    Laurel = (Laurel)c.Laurel,
                //    Level = c.Level,
                //    LastOnline = c.LastOnline,
                //    LastLocation = new Location(c.LastLocation.World, c.LastLocation.Guard, c.LastLocation.Section)
                //});

            }
            CharacterList = CharacterList.OrderBy(ch => ch.Position).ToList();
        }
    }

    //public class RawChar
    //{
    //    public uint Id;
    //    public uint GuildId;
    //    public int CharClass, Level;
    //    public string Name;
    //    internal int Pos;
    //    internal int Laurel;
    //    internal long LastOnline;
    //    internal Location LastLocation;
    //    public string GuildName = "";

    //    public override string ToString()
    //    {
    //        var sb = new StringBuilder();

    //        sb.AppendLine($"Character [{Pos}] <");
    //        sb.AppendLine($"\tName: {Name}");
    //        sb.AppendLine($"\tLevel: {Level}");
    //        sb.AppendLine($"\tClass: {(Class)CharClass}");
    //        sb.AppendLine($"\tLaurel: {(Laurel)Laurel}");
    //        sb.AppendLine(">");

    //        return sb.ToString();
    //    }
    //}
}