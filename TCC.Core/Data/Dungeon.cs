namespace TCC.Data
{
    public class Dungeon
    {
        //public string ShortName { get;  }
        public string Name { get; }
        public short MaxBaseRuns { get;  }
        public DungeonTier Tier { get;  }
        public uint Id { get; }
        public bool Show { get; }
        public Dungeon(uint id, string name, DungeonTier t, short r, bool show)
        {
            Id = id;
            //ShortName = n;
            MaxBaseRuns = r;
            Name = name;
            Tier = t;
            Show = show;
        }
    }
}
