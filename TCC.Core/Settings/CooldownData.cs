using TCC.Data;

namespace TCC.Settings
{
    public class CooldownData
    {
        public uint Id { get; }
        public CooldownType Type { get; }

        public CooldownData(uint id, CooldownType type)
        {
            Id = id;
            Type = type;
        }
    }
}