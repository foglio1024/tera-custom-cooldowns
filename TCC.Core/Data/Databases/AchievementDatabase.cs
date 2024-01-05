using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace TCC.Data.Databases;

public readonly record struct Achievement(uint Id, uint NameId, string Name);

public class AchievementDatabase : DatabaseBase
{
    readonly Dictionary<uint, Achievement> _achievements = [];
    readonly Dictionary<uint, Achievement> _achievementsByName = [];

    protected override string FolderName => "achievements";
    protected override string Extension => "tsv";

    public AchievementDatabase(string lang) : base(lang)
    {
    }

    public override void Load()
    {
        _achievements.Clear();
        _achievementsByName.Clear();

        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var s = line.Split('\t');

            if (!uint.TryParse(s[0], out var id)) continue;
            if (!uint.TryParse(s[1], out var nameid)) continue;
            
            var name = s[2];
            var achi = new Achievement(id, nameid, name);
            _achievements[id] = achi;
            _achievementsByName[nameid] = achi;
        }
    }

    public bool TryGetAchievementNameById(uint id, [NotNullWhen(true)] out string? name)
    {
        var ret = _achievements.TryGetValue(id, out var achi);
        if (!ret)
        {
            name = null;
            return false;
        }
        name = achi.Name;
        return true;
    }

    public bool TryGetAchievementNameByNameId(uint nameId, [NotNullWhen(true)] out string? name)
    {
        var ret = _achievementsByName.TryGetValue(nameId, out var achi);
        if (!ret)
        {
            name = null;
            return false;
        }
        name = achi.Name;
        return true;
    }

    public bool TryGetAchievementName(uint id, [NotNullWhen(true)] out string? o)
    {
        return TryGetAchievementNameById(id, out o) || TryGetAchievementNameByNameId(id, out o);
    }
}