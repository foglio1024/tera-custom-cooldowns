using System.Collections.Generic;

namespace TCC.Data.Map
{
    public class Guard
    {
        public Dictionary<uint, Section> Sections { get;  }
        public uint Id { get; }
        public uint NameId { get; }
        public uint ContinentId { get; }

        private Guard()
        {
            Sections = new Dictionary<uint, Section>();
        }

        public Guard(uint gId, uint gNameId, uint continentId) : this()
        {
            Id = gId;
            NameId = gNameId;
            ContinentId = continentId;
        }
    }
}
