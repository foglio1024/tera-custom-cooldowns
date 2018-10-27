namespace TCC.Data.Pc
{
    public class SimpleUser
    {
        public uint PlayerId { get; }
        public string Name { get; }

        public SimpleUser(uint id, string name)
        {
            PlayerId = id;
            Name = name;
        }
    }
}
