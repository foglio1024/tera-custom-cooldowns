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
                var id = uint.Parse(s[0]);
                var name = s[1];

                Achievements.Add(id, name);
            }
        }
    }
}
