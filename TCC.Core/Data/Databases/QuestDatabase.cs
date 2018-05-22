using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class QuestDatabase
    {
        public Dictionary<uint, string> Quests { get; }
        public QuestDatabase(string lang)
        {
            var f = File.OpenText($"resources/data/quests/quests-{lang}.tsv");
            Quests = new Dictionary<uint, string>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                if (!uint.TryParse(s[0], out var id)) continue;
                var name = s[1];

                Quests.Add(id, name);
            }
        }

    }
}
