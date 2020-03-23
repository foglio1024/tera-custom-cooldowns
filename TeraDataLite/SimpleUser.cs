namespace TeraDataLite
{
    public struct FriendData
    {
        public uint PlayerId { get; }
        public string Name { get; }

        public FriendData(uint id, string name)
        {
            PlayerId = id;
            Name = name;
        }
    }
}
