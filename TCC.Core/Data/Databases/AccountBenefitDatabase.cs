using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases;

public class AccountBenefitDatabase : DatabaseBase
{
    protected override string FolderName => "acc_benefits";
    protected override string Extension => "tsv";

    public readonly Dictionary<uint, string> Benefits = [];

    public AccountBenefitDatabase(string lang) : base(lang)
    {
    }

    public override void Load()
    {
        Benefits.Clear();
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var s = line.Split('\t');

            if (!uint.TryParse(s[0], out var val)) continue;

            Benefits.Add(val, s[1]);
        }
    }
}