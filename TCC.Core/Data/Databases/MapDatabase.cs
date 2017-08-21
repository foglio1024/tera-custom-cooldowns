using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace TCC.Data.Databases
{
    public static class MapDatabase
    {
        public static Dictionary<uint, World> Worlds;
        public static Dictionary<uint, string> Names;
        public static void Load()
        {
            Worlds = new Dictionary<uint, World>();
            Names = new Dictionary<uint, string>();

            var xdoc = XDocument.Load("resources/data/NewWorldMapData.xml");

            foreach (var w in xdoc.Descendants().Where(x => x.Name == "World"))
            {
                var wId = uint.Parse(w.Attribute("id").Value);
                var wNameId = w.Attribute("nameId") != null ? uint.Parse(w.Attribute("nameId").Value) : 0;
                var world = new World(wId, wNameId);

                foreach (var g in w.Descendants().Where(x => x.Name == "Guard"))
                {
                    var gId = uint.Parse(g.Attribute("id").Value);
                    var gNameId = g.Attribute("nameId") != null? uint.Parse(g.Attribute("nameId").Value) : 0;
                    var gMapId = g.Attribute("mapId") != null? g.Attribute("mapId").Value : "";
                    var guard = new Guard(gId, gNameId, gMapId);
                    
                    foreach (var s in g.Descendants().Where(x => x.Name == "Section"))
                    {
                        var sId = uint.Parse(s.Attribute("id").Value);
                        var sNameId = s.Attribute("nameId") != null ? uint.Parse(s.Attribute("nameId").Value) : 0;
                        var sTop = s.Attribute("top") != null ? Double.Parse(s.Attribute("top").Value, CultureInfo.InvariantCulture) : 0;
                        var sLeft = s.Attribute("left") != null ? Double.Parse(s.Attribute("left").Value, CultureInfo.InvariantCulture) : 0;
                        var sWidth = s.Attribute("width") != null ? Double.Parse(s.Attribute("width").Value, CultureInfo.InvariantCulture) : 0;
                        var sMapId = s.Attribute("mapId") != null ? s.Attribute("mapId").Value : "";
                        var dg = s.Attribute("type") != null && s.Attribute("type").Value == "dungeon" ? true : false;
                        var section = new Section(sId, sNameId, sMapId, sTop, sLeft, sWidth, dg);

                        guard.Sections.Add(sId, section);
                    }
                    world.Guards.Add(guard.Id, guard);
                }
                Worlds.Add(world.Id, world);
            }

            LoadNames();
        }

        internal static bool GetDungeon(Location loc)
        {
            if (loc.World == 9999) return true;
            return Worlds[loc.World].Guards[loc.Guard].Sections[loc.Section].IsDungeon;
        }

        internal static Point GetMarkerPosition(Location loc)
        {
            var section = Worlds[loc.World].Guards[loc.Guard].Sections[loc.Section];
            var offset = new Point(section.Left, section.Top);
            return new Point((offset.Y - loc.Position.X)/section.Scale, (- offset.X + loc.Position.Y) /section.Scale);
            
        }

        static void LoadNames()
        {
            var f = File.OpenText(Environment.CurrentDirectory + "/resources/data/regions.tsv");
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                var id = Convert.ToUInt32(s[0]);
                var name = s[1];

                Names.Add(id, name);
            }

        }

        public static string GetMapId(uint w, uint g, uint s)
        {
            return Worlds[w].Guards[g].Sections[s].MapId;
        }
    }


}
