namespace TCC.Data
{
    public class Item
    {
        public uint Id { get; }
        public uint ExpId { get; }
        public string Name { get; }
        public RareGrade RareGrade { get; }
        public uint Cooldown { get; }
        public string IconName { get; }

        public Item(uint id, string name, RareGrade g, uint expId, uint cd, string iconName)
        {
            Id = id;
            Name = name;
            RareGrade = g;
            ExpId = expId;
            Cooldown = cd;
            IconName = iconName;
        }
    }
}