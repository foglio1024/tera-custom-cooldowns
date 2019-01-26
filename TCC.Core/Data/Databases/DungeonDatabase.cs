using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TCC.Data.Databases
{
    public class DungeonDatabase : DatabaseBase
    {
        private readonly string CustomDefsPath = Path.Combine(App.DataPath, "dungeon-defs.tsv");
        private readonly string DefaultDefsFullPath = Path.Combine(App.DataPath, "default-dungeon-defs.tsv");
        private readonly string DefaultDefsRelativePath =  "default-dungeon-defs.tsv";
        private readonly string ImagesFullPath = Path.Combine(App.DataPath, "section_images.tsv");
        private readonly string ImagesRelativePath = "section_images.tsv";

        public readonly Dictionary<uint, Dungeon> Dungeons;

        protected override string FolderName => "dungeons";
        protected override string Extension => "tsv";
        public override bool Exists => base.Exists
                                    && File.Exists(DefaultDefsFullPath) 
                                    && File.Exists(ImagesFullPath);

        public override void CheckVersion(string customAbsPath = null, string customRelPath = null)
        {
            base.CheckVersion();
            base.CheckVersion(DefaultDefsFullPath, DefaultDefsRelativePath);
            base.CheckVersion(ImagesFullPath, ImagesRelativePath);
        }
        public override void Update(string custom = null)
        {
            base.Update();
            base.Update(DefaultDefsRelativePath);
            base.Update(ImagesRelativePath);
        }

        public DungeonDatabase(string lang) :base(lang)
        {
            Dungeons = new Dictionary<uint, Dungeon>();
        }



        private void ParseDungeons()
        {
            var f = File.OpenText(FullPath);
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
            if (!File.Exists(CustomDefsPath)) File.Copy(DefaultDefsFullPath, CustomDefsPath);
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
                if (!Dungeons.TryGetValue(id, out var dung)) continue;
                dung.Show = true;
                dung.ShortName = shortName;
                dung.DoublesOnElite = bool.Parse(sDobuleElite);
                dung.Index = int.Parse(sIndex);
                if (short.TryParse(sMaxBaseRuns, out var baseRuns)) dung.MaxBaseRuns = baseRuns;
                if (!int.TryParse(sReqIlvl, out var reqIlvl)) continue;
                try
                {
                    dung.RequiredIlvl = (ItemLevelTier)reqIlvl;
                }
                catch
                {

                }
            }

        }
        private void ParseDungeonIcons()
        {
            var f = File.OpenText(ImagesFullPath);
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var split = line.Split('\t');

                var id = Convert.ToUInt32(split[2]);
                var icon = split[3];

                if (Dungeons.TryGetValue(id, out var dung)) dung.IconName = icon;
            }
        }

        public string GetDungeonNameOrOpenWorld(uint continentId)
        {
            return Dungeons.TryGetValue(continentId, out var dungeon) ? dungeon.Name : "Open world";
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

        public override void Load()
        {
            Dungeons.Clear();
            ParseDungeons();
            ParseDungeonDefs();
            ParseDungeonIcons();
        }
    }
}
