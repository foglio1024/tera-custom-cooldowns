using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_OTHER_USER_APPLY_PARTY : ParsedMessage
{
    public bool IsRaid { get; }
    public uint PlayerId { get; }
    public uint ServerId { get; }
    public Class Class { get; }
    public short Level { get; }
    public string Name { get; }

    public S_OTHER_USER_APPLY_PARTY(TeraMessageReader reader) : base(reader)
    {
        if (reader.Factory.ReleaseVersion / 100 >= 108)
        {
            var nameOffset = reader.ReadUInt16();
            IsRaid = reader.ReadBoolean();
            PlayerId = reader.ReadUInt32();
            ServerId = reader.ReadUInt32();
            Class = (Class)reader.ReadUInt16();
            reader.Skip(2 + 2); // race, gender
            Level = reader.ReadInt16();
            reader.RepositionAt(nameOffset);
            Name = reader.ReadTeraString();
        }
        else
        {
            var nameOffset = reader.ReadUInt16();
            reader.Skip(1); // isRaid?
            PlayerId = reader.ReadUInt32();
            Class = (Class)reader.ReadInt16();
            reader.Skip(4); //race, gender
            Level = reader.ReadInt16();
            reader.RepositionAt(nameOffset);
            // bool isWorldEventTarget?
            Name = reader.ReadTeraString();
        }
    }
}