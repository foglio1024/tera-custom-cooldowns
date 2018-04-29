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
        public static DungeonDatabase Instance => _instance ?? (_instance = new DungeonDatabase(SettingsManager.LastRegion));
        public Dictionary<uint, Dungeon> Dungeons;
        //public Dictionary<uint, Dungeon> DungeonDefinitions;
        public DungeonDatabase(string lang)
        {
            if (string.IsNullOrEmpty(lang)) lang = "EU-EN";
            var f = File.OpenText($"resources/data/dungeons/dungeons-{lang}.tsv");
            //DungeonNames = new Dictionary<uint, string>();
            Dungeons = new Dictionary<uint, Dungeon>();
            //TODO
            var defs = new Dictionary<uint, Tuple<short, DungeonTier>>();
            var def = XDocument.Load("resources/data/dungeons-def.xml");
            foreach (var dg in def.Descendants().Where(x => x.Name == "Dungeon"))
            {
                var id = Convert.ToUInt32(dg.Attribute("Id").Value);
                var r = Convert.ToInt16(dg.Attribute("MaxBaseRuns").Value);
                //var n = dg.Attribute("ShortName").Value;
                var t = (DungeonTier)Enum.Parse(typeof(DungeonTier), dg.Attribute("Tier").Value);
                defs.Add(id, new Tuple<short, DungeonTier>(r, t));
            }
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = uint.Parse(s[0]);
                var name = s[1];
                //var t = (DungeonTier)Enum.Parse(typeof(DungeonTier), s[2]);

                if (defs.ContainsKey(id))
                {
                    var dg = new Dungeon(id, name, defs[id].Item2, defs[id].Item1, true);
                    Dungeons.Add(id, dg);
                }
                else
                {
                    //var dg = new Dungeon(id, name, t, 0, false);
                }
            }
        }

        internal static void Reload(string language)
        {
            _instance = new DungeonDatabase(language);
        }

        public string GetDungeonNameOrOpenWorld(uint continentId)
        {
            return Dungeons.ContainsKey(continentId) ? Dungeons[continentId].Name : "Open world";
        }
    }
}
