using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TCC.Data.Databases;

public class DungeonDatabase : DatabaseBase
{
    private const string DefaultDefsRelativePath = "default-dungeon-defs.tsv";
    private const string ImagesRelativePath = "section_images.tsv";

    protected override string FolderName => "dungeons";
    protected override string Extension => "tsv";

    private readonly string _customDefsPath = Path.Combine(App.DataPath, "dungeon-defs.tsv");
    private readonly string _defaultDefsFullPath = Path.Combine(App.DataPath, "default-dungeon-defs.tsv");
    private readonly string _imagesFullPath = Path.Combine(App.DataPath, "section_images.tsv");

    public readonly Dictionary<uint, Dungeon> Dungeons = [];

    public override bool Exists => base.Exists
                                   && File.Exists(_defaultDefsFullPath)
                                   && File.Exists(_imagesFullPath);

    public DungeonDatabase(string lang) : base(lang)
    {
    }

    public override void CheckVersion(string customAbsPath = "", string customRelPath = "")
    {
        base.CheckVersion(FullPath, RelativePath);
        base.CheckVersion(_defaultDefsFullPath, DefaultDefsRelativePath);
        base.CheckVersion(_imagesFullPath, ImagesRelativePath);
    }

    public override void Update(string custom = "")
    {
        base.Update(RelativePath);
        base.Update(DefaultDefsRelativePath);
        base.Update(ImagesRelativePath);
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
        foreach (var d in Dungeons.Values.Where(d => d.Index != -1 && d.Show))
        {
            sb.Append(d.Id);
            sb.Append("\t");
            sb.Append(d.ShortName);
            sb.Append("\t");
            sb.Append(d.MaxBaseRuns);
            sb.Append("\t");
            sb.Append(d.ItemLevel);
            sb.Append("\t");
            sb.Append(d.DoublesOnElite);
            sb.Append("\t");
            sb.Append(d.Index);
            sb.Append("\t");
            sb.Append(d.ResetMode);
            sb.Append("\n");
        }

        File.WriteAllText(_customDefsPath, sb.ToString());
    }

    public override void Load()
    {
        Dungeons.Clear();
        ParseDungeons();
        ParseDungeonDefs();
        ParseDungeonIcons();
    }

    private void ParseDungeons()
    {
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var s = line.Split('\t');
            var id = uint.Parse(s[0]);
            var name = s[1];
            var dg = new Dungeon(id, name);
            if (s.Length > 2) //TODO: remove check when all regions are updated
            {
                dg.Cost = int.Parse(s[2]);
            }
            Dungeons[id] = dg;
        }
    }

    private void ParseDungeonDefs()
    {
        if (!File.Exists(_customDefsPath)) File.Copy(_defaultDefsFullPath, _customDefsPath);
        var lines = File.ReadAllLines(_customDefsPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;
            if (line.StartsWith("#")) continue;

            var s = line.Split('\t');
            var id = uint.Parse(s[0]);
            var shortName = s[1];
            var sMaxBaseRuns = s[2];
            var sReqIlvl = s[3];
            var sDobuleElite = s[4];
            var sIndex = s[5];
            var resetMode = ResetMode.Daily;
            if (s.Length >= 7 && (ResetMode)Enum.Parse(typeof(ResetMode), s[6]) is ResetMode.Weekly)
            {
                resetMode = ResetMode.Weekly;
            }

            if (!Dungeons.TryGetValue(id, out var dung)) continue;

            dung.Show = true;
            dung.HasDef = true;
            dung.ShortName = shortName;
            dung.DoublesOnElite = bool.Parse(sDobuleElite);
            dung.Index = int.Parse(sIndex);
            dung.ResetMode = resetMode;
            if (short.TryParse(sMaxBaseRuns, out var baseRuns))
            {
                dung.MaxBaseRuns = baseRuns;
            }

            if (!int.TryParse(sReqIlvl, out var reqIlvl)) continue;

            try
            {
                dung.ItemLevel = reqIlvl;
            }
            catch
            {
                // ignored
            }
        }
    }

    private void ParseDungeonIcons()
    {
        var lines = File.ReadAllLines(_imagesFullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var split = line.Split('\t');
            var id = Convert.ToUInt32(split[2]);
            var icon = split[3];
            if (Dungeons.TryGetValue(id, out var dung))
            {
                dung.IconName = icon;
            }
        }
    }
}