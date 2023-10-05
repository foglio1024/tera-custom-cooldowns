using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases;

public class GuildQuestDatabase : DatabaseBase
{
    public readonly Dictionary<uint, GuildQuest> GuildQuests;

    protected override string FolderName => "guild_quests";

    protected override string Extension => "tsv";

    public GuildQuestDatabase(string lang) : base(lang)
    {
        GuildQuests = new Dictionary<uint, GuildQuest>();
    }

    public override void Load()
    {
        GuildQuests.Clear();
        //var f = File.OpenText(FullPath);
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            //var line = f.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) break;
            var s = line.Split('\t');
            var id = uint.Parse(s[0]);
            var str = s[1];
            //var zId = uint.Parse(s[2]);
            GuildQuests.Add(id, new GuildQuest(id, str));
        }
    }
}