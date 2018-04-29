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
        public static void Load(string lang)
        {
            Worlds = new Dictionary<uint, World>();
            Names = new Dictionary<uint, string>();

            var xdoc = XDocument.Load($"resources/data/world_map/world_map-{lang}.xml");

            foreach (var w in xdoc.Descendants().Where(x => x.Name == "World"))
            {
                var wId = uint.Parse(w.Attribute("id").Value);
                var wNameId = w.Attribute("nameId") != null ? uint.Parse(w.Attribute("nameId").Value) : 0;
                var world = new World(wId, wNameId);

                foreach (var g in w.Descendants().Where(x => x.Name == "Guard"))
                {
                    var gId = uint.Parse(g.Attribute("id").Value);
                    var gNameId = g.Attribute("nameId") != null ? uint.Parse(g.Attribute("nameId").Value) : 0;
                    var gMapId = g.Attribute("mapId") != null ? g.Attribute("mapId").Value : "";
                    //var gTop = g.Attribute("top") != null ? Double.Parse(g.Attribute("top").Value, CultureInfo.InvariantCulture) : 0;
                    //var gLeft = g.Attribute("left") != null ? Double.Parse(g.Attribute("left").Value, CultureInfo.InvariantCulture) : 0;
                    //var gWidth = g.Attribute("width") != null ? Double.Parse(g.Attribute("width").Value, CultureInfo.InvariantCulture) : 0;
                    //var gHeight = g.Attribute("height") != null ? Double.Parse(g.Attribute("height").Value, CultureInfo.InvariantCulture) : 0;

                    var guard = new Guard(gId, gNameId, gMapId/*, gLeft, gTop, gWidth, gHeight*/);
                    guard.ContinentId = g.Attribute("continentId") != null ? Convert.ToUInt32(g.Attribute("continentId").Value) : 0;

                    foreach (var s in g.Descendants().Where(x => x.Name == "Section"))
                    {
                        var sId = uint.Parse(s.Attribute("id").Value);
                        var sNameId = s.Attribute("nameId") != null ? uint.Parse(s.Attribute("nameId").Value) : 0;
                        //var sTop = s.Attribute("top") != null ? Double.Parse(s.Attribute("top").Value, CultureInfo.InvariantCulture) : 0;
                        //var sLeft = s.Attribute("left") != null ? Double.Parse(s.Attribute("left").Value, CultureInfo.InvariantCulture) : 0;
                        //var sWidth = s.Attribute("width") != null ? Double.Parse(s.Attribute("width").Value, CultureInfo.InvariantCulture) : 0;
                        //var sHeight = s.Attribute("height") != null ? Double.Parse(s.Attribute("height").Value, CultureInfo.InvariantCulture) : 0;
                        var sMapId = s.Attribute("mapId") != null ? s.Attribute("mapId").Value : "";
                        var dg = s.Attribute("type") != null && s.Attribute("type").Value == "dungeon" ? true : false;
                        //var cId = s.Descendants().Any()? uint.Parse(s.Descendants().FirstOrDefault(x => x.Name == "Npc").Attribute("continentId").Value) : 0;

                        var section = new Section(sId, sNameId, sMapId/*, sTop, sLeft, sWidth, sHeight*/, dg);
                        guard.Sections.Add(sId, section);
                    }
                    world.Guards.Add(guard.Id, guard);
                }
                Worlds.Add(world.Id, world);
            }
            LoadNames(lang);
        }

        public static bool TryGetGuardOrDungeonNameFromContinentId(uint continent, out string s)
        {
            if (DungeonDatabase.Instance.Dungeons.ContainsKey(continent))
            {
                s = DungeonDatabase.Instance.Dungeons[continent].Name;
                return true;
            }
            var guard = Worlds[1].Guards.FirstOrDefault(x => x.Value.ContinentId == continent);
            if (guard.Value == null)
            {
                s = "Unknown";
                return false;
            }
            s = Names[guard.Value.NameId];
            return true;

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
            return new Point((offset.Y - loc.Position.X) / section.Scale, (-offset.X + loc.Position.Y) / section.Scale);

        }

        static void LoadNames(string lang)
        {
            var f = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + $"/resources/data/regions/regions-{lang}.tsv");
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

        public static string GetName(uint guardId, uint sectionId)
        {
            var ret = "Unknown;";
            try
            {
                Worlds.ToList().ForEach(w =>
                {
                    if (w.Value.Guards.ContainsKey(guardId))
                    {
                        var g = w.Value.Guards[guardId];
                        if (g.Sections.ContainsKey(sectionId))
                        {
                            var name = g.Sections[sectionId].NameId;
                            ret = Names[name];
                            return;
                        }
                    }
                });
            }
            catch (Exception)
            {
            }
            return ret;
        }
    }


}
