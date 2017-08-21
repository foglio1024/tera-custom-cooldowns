using System.Collections.Generic;

namespace TCC.Data
{
    public class Guard
    {
        public Dictionary<uint, Section> Sections { get; set; }
        public uint Id { get; }
        public uint NameId { get; }
        public string MapId { get; }
        public Guard(uint gId, uint gNameId, string mapId)
        {
            Sections = new Dictionary<uint, Section>();
            Id = gId;
            NameId = gNameId;
            MapId = mapId;
        }
    }
}
