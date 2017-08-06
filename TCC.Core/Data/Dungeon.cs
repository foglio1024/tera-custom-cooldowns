using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data
{
    public class Dungeon
    {
        public string ShortName { get;  }
        public short MaxBaseRuns { get;  }
        public DungeonTier Tier { get;  }
        public uint Id { get; }
        public Dungeon(uint id, string n, short r, DungeonTier t)
        {
            Id = id;
            ShortName = n;
            MaxBaseRuns = r;
            Tier = t;
        }
    }
}
