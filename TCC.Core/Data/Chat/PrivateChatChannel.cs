namespace TCC.Data.Chat
{
    public struct PrivateChatChannel
    {
        public uint Id;
        public string Name;
        public int Index;
        public bool Joined;
        public PrivateChatChannel(uint id, string name, int index)
        {
            Id = id;
            Name = name;
            Index = index;
            Joined = true;
        }
    }
}
