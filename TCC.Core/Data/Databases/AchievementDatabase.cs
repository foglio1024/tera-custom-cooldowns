using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases;

public class AchievementDatabase : DatabaseBase
{
    public readonly Dictionary<uint, string> Achievements;
    public AchievementDatabase(string lang) : base(lang)
    {
        Achievements = new Dictionary<uint, string>();
    }

    protected override string FolderName => "achievements";

    protected override string Extension => "tsv";

    public override void Load()
    {
        Achievements.Clear();
        //var f = File.OpenText(FullPath);
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            //var line = f.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) break;
            var s = line.Split('\t');
            if (!uint.TryParse(s[0], out var id)) continue;
            var name = s[1];

            Achievements.Add(id, name);
        }
    }
}