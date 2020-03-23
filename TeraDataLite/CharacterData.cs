namespace TeraDataLite
{
    public struct CharacterData
    {
        public uint Id { get; set; }
        public Class CharClass { get; set; }
        public int Level { get; set; }
        public uint LastWorldId { get; set; }
        public uint LastGuardId { get; set; }
        public uint LastSectionId { get; set; }
        public long LastOnline { get; set; }
        public Laurel Laurel { get; set; }
        public int Position { get; set; }
        public uint GuildId { get; set; }
        public string Name { get; set; }
        public string GuildName { get; set; }
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