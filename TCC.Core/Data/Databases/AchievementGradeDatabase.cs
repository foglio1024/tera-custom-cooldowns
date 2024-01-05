using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases;

public class AchievementGradeDatabase : DatabaseBase
{
    protected override string FolderName => "achi_grade";
    protected override string Extension => "tsv";

    public Dictionary<uint, string> Grades { get; } = [];

    public AchievementGradeDatabase(string lang) : base(lang)
    {
    }

    public override void Load()
    {
        Grades.Clear();
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var s = line.Split('\t');

            if (!uint.TryParse(s[0], out var id)) continue;

            var name = s[1];
            Grades.Add(id, name);
        }
    }
}