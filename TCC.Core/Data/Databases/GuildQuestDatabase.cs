using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class GuildQuestDatabase
    {
        public Dictionary<uint, GuildQuest> GuildQuests { get; }
        public GuildQuestDatabase(string lang)
        {
            GuildQuests = new Dictionary<uint, GuildQuest>();
            var f = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + $"/resources/data/guild_quests/guild_quests-{lang}.tsv");
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = uint.Parse(s[0]);
                var str = s[1];
                //var zId = uint.Parse(s[2]);
                GuildQuests.Add(id, new GuildQuest(id, str));
            }
        }
    }
    public class GuildQuest
    {
        public uint Id { get; private set; }
        public string Title { get; private set; }
        public uint ZoneId { get; private set; }

        public GuildQuest(uint id, string s)
        {
            Id = id;
            Title = s;
            //ZoneId = zId;
        }
    }

}
