namespace TCC.Data
{
    public class Item
    {
        public uint Id { get; private set; }
        public uint ExpId { get; private set; }
        public string Name { get; private set; } = "";
        public RareGrade RareGrade { get; private set; }
        public uint Cooldown { get; private set; }
        public string IconName { get; private set; } = "";
        public Item(uint id, string name, uint g, uint expId, uint cd, string iconName)
        {
            Id = id;
            Name = name;
            RareGrade = (RareGrade)g;
            ExpId = expId;
            Cooldown = cd;
            IconName = iconName;
        }
    }
}