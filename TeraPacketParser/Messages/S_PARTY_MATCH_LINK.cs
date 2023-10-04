using Nostrum.Extensions;
using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_PARTY_MATCH_LINK : ParsedMessage
{
    public uint Id { get; private set; }
    public bool Raid { get; private set; }
    public string Name { get; private set; }
    public string Message { get; private set; }
    public ListingData ListingData { get; }

    public S_PARTY_MATCH_LINK(TeraMessageReader reader) : base(reader)
    {
        var nameOffset = reader.ReadUInt16();
        var msgOffset = reader.ReadUInt16();

        Id = reader.ReadUInt32();
        var serverId = 0U;
        if (reader.Factory.ReleaseVersion / 100 >= 108) serverId = reader.ReadUInt32();
        reader.Skip(1);
        Raid = reader.ReadBoolean();

        reader.BaseStream.Position = nameOffset - 4;
        Name = reader.ReadTeraString();

        reader.BaseStream.Position = msgOffset - 4;
        Message = reader.ReadTeraString().UnescapeHtml();

        ListingData = new ListingData
        {
            LeaderId = Id,
            LeaderServerId = serverId,
            IsRaid = Raid,
            Message = Message,
            LeaderName = Name
        };
    }
}