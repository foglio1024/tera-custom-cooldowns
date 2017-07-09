using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    var guard = new Guard(gId, gNameId);
                    
                    foreach (var s in g.Descendants().Where(x => x.Name == "Section"))
                    {
                        var sId = uint.Parse(s.Attribute("id").Value);
                        var sNameId = s.Attribute("nameId") != null ? uint.Parse(s.Attribute("nameId").Value) : 0;
                        var section = new Section(sId, sNameId);

                        guard.Sections.Add(sId, section);
                    }
                    world.Guards.Add(guard.Id, guard);
                }
                Worlds.Add(world.Id, world);
            }

            LoadNames();
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
    }


}
