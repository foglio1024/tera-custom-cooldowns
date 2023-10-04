namespace TeraPacketParser.Messages;

public class S_USER_STATUS : ParsedMessage
{
    public enum UserStatus
    {
        Normal = 0,
        InCombat = 1,
        Campfire = 2,
        OnPegasus = 3
    }
    public ulong GameId { get; private set; }
    public UserStatus Status {  get; private set; }

    public S_USER_STATUS(TeraMessageReader reader) : base(reader)
    {
        GameId = reader.ReadUInt64();
        Status = (UserStatus)reader.ReadUInt32();
    }
}