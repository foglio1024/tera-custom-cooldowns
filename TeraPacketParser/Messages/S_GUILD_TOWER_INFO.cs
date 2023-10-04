


namespace TeraPacketParser.Messages;

public class S_GUILD_TOWER_INFO : ParsedMessage
{
    public ulong TowerId { get; set; }
    public string GuildName { get; set; }
    public uint GuildId { get; set; }
    public string LogoInfo { get; set; }
    public S_GUILD_TOWER_INFO(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(2);
        var nameOffset = reader.ReadUInt16();
        TowerId = reader.ReadUInt64();
        GuildId = reader.ReadUInt32();
        LogoInfo = reader.ReadTeraString();
        reader.BaseStream.Position = nameOffset - 4;
        GuildName = reader.ReadTeraString();
    }
}