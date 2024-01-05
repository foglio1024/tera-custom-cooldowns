using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases;

public class SocialDatabase : DatabaseBase
{
    public Dictionary<uint, string> Social { get; } = [];

    protected override string FolderName => "social";
    protected override string Extension => "tsv";

    public SocialDatabase(string lang) : base(lang)
    {
    }

    public override void Load()
    {
        Social.Clear();
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var s = line.Split('\t');
            var id = Convert.ToUInt32(s[0]);
            var phrase = s[1];
            Social.Add(id, phrase);
        }
    }
}