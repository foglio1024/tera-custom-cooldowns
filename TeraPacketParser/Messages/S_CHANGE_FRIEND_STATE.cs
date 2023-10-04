using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_CHANGE_FRIEND_STATE : ParsedMessage
{
    public uint PlayerId { get; }
    public FriendStatus FriendStatus { get; set; }
    public S_CHANGE_FRIEND_STATE(TeraMessageReader reader) : base(reader)
    {
        PlayerId = reader.ReadUInt32();
        FriendStatus = (FriendStatus)reader.ReadUInt32();
    }
}