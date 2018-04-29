using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public static class AchievementDatabase
    {
        public static Dictionary<uint, string> Achievements;
        public static void Load(string lang)
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
