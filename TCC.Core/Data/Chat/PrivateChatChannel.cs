namespace TCC.Data.Chat;

public struct PrivateChatChannel
{
    public readonly uint Id;
    public readonly string Name;
    public readonly int Index;
    public bool Joined;

    public PrivateChatChannel(uint id, string name, int index)
    {
        Id = id;
        Name = name;
        Index = index;
        Joined = true;
    }
}