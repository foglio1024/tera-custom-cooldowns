using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class AchievementDatabase
    {
        public Dictionary<uint, string> Achievements { get; }
        public AchievementDatabase(string lang)
        {
            var f = File.OpenText($"resources/data/achievements/achievements-{lang}.tsv");
            Achievements = new Dictionary<uint, string>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                if (!uint.TryParse(s[0], out var id)) continue;
                var name = s[1];

                Achievements.Add(id, name);
            }
        }
    }
}
