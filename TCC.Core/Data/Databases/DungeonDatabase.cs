using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TCC.Data.Databases
{
    public class DungeonDatabase
    {
        private readonly string CustomDefsPath = Path.Combine(App.DataPath, "dungeon-defs.tsv");
        private readonly string DefaultDefsPath = Path.Combine(App.DataPath, "default-dungeon-defs.tsv");
        private readonly string ImagesPath = Path.Combine(App.DataPath, "section_images.tsv");
        public readonly Dictionary<uint, Dungeon> Dungeons;

        public DungeonDatabase(string lang)
        {
            Dungeons = new Dictionary<uint, Dungeon>();
            ParseDungeons(lang);
            ParseDungeonDefs();
            ParseDungeonIcons();
        }

        private void ParseDungeons(string lang)
        {
            if (string.IsNullOrEmpty(lang)) lang = "EU-EN";
            var f = File.OpenText(Path.Combine(App.DataPath, $"dungeons/dungeons-{lang}.tsv"));
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = uint.Parse(s[0]);
                var name = s[1];
                Dungeons[id] = new Dungeon(id, name);
            }
        }
        private void ParseDungeonDefs()
        {
            if (!File.Exists(CustomDefsPath))
                File.Copy(DefaultDefsPath, CustomDefsPath);
            var def = File.OpenText(CustomDefsPath);
            while (true)
            {
                var line = def.ReadLine();
                if (line == null) break;
                if (line.StartsWith("#")) continue;
                var s = line.Split('\t');
                var id = uint.Parse(s[0]);
                var shortName = s[1];
                var sMaxBaseRuns = s[2];
                var sReqIlvl = s[3];
                var sDobuleElite = s[4];
                var sIndex = s[5];
                if (!Dungeons.ContainsKey(id)) continue;
                Dungeons[id].Show = true;
                Dungeons[id].ShortName = shortName;
                Dungeons[id].DoublesOnElite = bool.Parse(sDobuleElite);
                Dungeons[id].Index = int.Parse(sIndex);
                if (short.TryParse(sMaxBaseRuns, out var baseRuns)) Dungeons[id].MaxBaseRuns = baseRuns;
                if (!int.TryParse(sReqIlvl, out var reqIlvl)) continue;
                try
                {
                    Dungeons[id].RequiredIlvl = (ItemLevelTier)reqIlvl;
                }
                catch
                {

                }
            }

        }
        private void ParseDungeonIcons()
        {
            var f = File.OpenText(ImagesPath);
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var split = line.Split('\t');

                var id = Convert.ToUInt32(split[2]);
                var icon = split[3];

                if (Dungeons.ContainsKey(id)) Dungeons[id].IconName = icon;
            }
        }

        public string GetDungeonNameOrOpenWorld(uint continentId)
        {
            return Dungeons.ContainsKey(continentId) ? Dungeons[continentId].Name : "Open world";
        }

        public void UpdateIndex(uint dungeonId, int newIdx)
        {
            Dungeons[dungeonId].Index = newIdx;
        }

        public void SaveCustomDefs()
        {
            var sb = new StringBuilder();
            Dungeons.Values.Where(d => d.Index != -1 && d.Show).ToList().ForEach(d =>
            {
                sb.Append(d.Id);
                sb.Append("\t");
                sb.Append(d.ShortName);
                sb.Append("\t");
                sb.Append(d.MaxBaseRuns);
                sb.Append("\t");
                sb.Append((int)d.RequiredIlvl);
                sb.Append("\t");
                sb.Append(d.DoublesOnElite);
                sb.Append("\t");
                sb.Append(d.Index);
                sb.Append("\n");
            });
            File.WriteAllText(CustomDefsPath, sb.ToString());
        }
    }
}
