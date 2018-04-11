using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Databases
{
    public class DungeonDatabase
    {
        static DungeonDatabase _instance;
        public static DungeonDatabase Instance => _instance ?? (_instance = new DungeonDatabase());
        public Dictionary<uint, string> DungeonNames;
        public Dictionary<uint, Dungeon> DungeonDefinitions;
        public DungeonDatabase()
        {
            var f = File.OpenText("resources/data/dungeons.tsv");
            DungeonNames = new Dictionary<uint, string>();
            DungeonDefinitions = new Dictionary<uint, Dungeon>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = UInt32.Parse(s[0]);
                var name = s[1];

                DungeonNames.Add(id, name);
            }
            var def = XDocument.Load("resources/data/dungeons-def.xml");
            foreach (var dg in def.Descendants().Where(x => x.Name == "Dungeon"))
            {
                var id = Convert.ToUInt32(dg.Attribute("Id").Value);
                var r = Convert.ToInt16(dg.Attribute("MaxBaseRuns").Value);
                var n = dg.Attribute("ShortName").Value;
                var t = (DungeonTier)Enum.Parse(typeof(DungeonTier), dg.Attribute("Tier").Value);
                var dgDef = new Dungeon(id, n, r, t);
                DungeonDefinitions.Add(id, dgDef);
            }
        }

        public string GetDungeonNameOrOpenWorld(uint continentId)
        {
            return DungeonNames.ContainsKey(continentId) ? DungeonNames[continentId] : "Open world";
        }
    }
}
