using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases;

public class GuildQuestDatabase : DatabaseBase
{
    protected override string FolderName => "guild_quests";
    protected override string Extension => "tsv";

    public readonly Dictionary<uint, GuildQuest> GuildQuests = [];

    public GuildQuestDatabase(string lang) : base(lang)
    {
    }

    public override void Load()
    {
        GuildQuests.Clear();
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var s = line.Split('\t');
            var id = uint.Parse(s[0]);
            var str = s[1];
            GuildQuests.Add(id, new GuildQuest(id, str));
        }
    }
}