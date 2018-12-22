using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public bool GetDungeon(Location loc)
        {
            if (loc.World == 9999) return true;
            return Worlds[loc.World].Guards[loc.Guard].Sections[loc.Section].IsDungeon;
        }

        public string GetMapId(uint w, uint g, uint s)
        {
            return Worlds[w].Guards[g].Sections[s].MapId;
        }

        public override void Load()
        {
            Worlds.Clear();
            var xdoc = XDocument.Load(FullPath);

            foreach (var w in xdoc.Descendants().Where(x => x.Name == "World"))
            {
                var wId = uint.Parse(w.Attribute("id")?.Value);
                var wNameId = w.Attribute("nameId") != null ? uint.Parse(w.Attribute("nameId").Value) : 0;
                var world = new World(wId, wNameId);

                foreach (var g in w.Descendants().Where(x => x.Name == "Guard"))
                {
                    var gId = uint.Parse(g.Attribute("id").Value);
                    var gNameId = g.Attribute("nameId") != null ? uint.Parse(g.Attribute("nameId").Value) : 0;
                    var gMapId = g.Attribute("mapId") != null ? g.Attribute("mapId").Value : "";

                    var guard = new Guard(gId, gNameId, gMapId)
                    {
                        ContinentId = g.Attribute("continentId") != null
                            ? Convert.ToUInt32(g.Attribute("continentId").Value)
                            : 0
                    };

                    foreach (var s in g.Descendants().Where(x => x.Name == "Section"))
                    {
                        var sId = uint.Parse(s.Attribute("id").Value);
                        var sNameId = s.Attribute("nameId") != null ? uint.Parse(s.Attribute("nameId").Value) : 0;
                        var sMapId = s.Attribute("mapId") != null ? s.Attribute("mapId").Value : "";
                        var dg = s.Attribute("type") != null && s.Attribute("type").Value == "dungeon";
                        var section = new Section(sId, sNameId, sMapId, dg);
                        guard.Sections.Add(sId, section);
                    }
                    world.Guards.Add(guard.Id, guard);
                }
                Worlds.Add(world.Id, world);
            }
        }
    }
}
