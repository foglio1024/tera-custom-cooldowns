using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TCC.Data.Map;

namespace TCC.Data.Databases
{
    public class MapDatabase : DatabaseBase
    {
        public Dictionary<uint, World> Worlds { get; }

        protected override string FolderName => "world_map";
        protected override string Extension => "xml";


        public MapDatabase(string lang) : base(lang)
        {
            Worlds = new Dictionary<uint, World>();
        }

        public bool IsDungeon(Location loc)
        {
            return loc.World == 9999 || Worlds[loc.World].Guards[loc.Guard].Sections[loc.Section].IsDungeon;
        }

        public string GetMapId(uint w, uint g, uint s)
        {
            return Worlds[w].Guards[g].Sections[s].MapId;
        }


        public override void Load()
        {
            Worlds.Clear();
            var xdoc = XDocument.Load(FullPath);

            xdoc.Descendants().Where(x => x.Name == "World").ToList().ForEach(worldElem =>
            {
                var world = World.FromXElement(worldElem);
                Worlds[world.Id] = world;
            });

        }
    }
}
