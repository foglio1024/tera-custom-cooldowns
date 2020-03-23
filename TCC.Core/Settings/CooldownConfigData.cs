using System.Collections.Generic;

namespace TCC.Settings
{
    public class CooldownConfigData
    {
        public List<CooldownData> Main { get; }
        public List<CooldownData> Secondary { get; }
        public List<CooldownData> Hidden { get; }

        public CooldownConfigData()
        {
            Main = new List<CooldownData>();
            Secondary = new List<CooldownData>();
            Hidden = new List<CooldownData>();
        }
    }
}
