using TeraDataLite;

namespace TeraPacketParser.Messages;

public class C_LOGIN_ARBITER : ParsedMessage
{
    public string AccountName { get; }
    public LangEnum Language { get; set; }
    public int Version { get; set; }

    public C_LOGIN_ARBITER(TeraMessageReader reader) : base(reader)
    {
        var nameOffset = reader.ReadUInt16();
        reader.Skip(9);
        Language = (LangEnum)reader.ReadUInt32();
        Version = reader.ReadInt32();
        reader.RepositionAt(nameOffset);
        AccountName = reader.ReadTeraString();
        //reader.Factory.ReleaseVersion = Version;
    }

}