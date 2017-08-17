using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data.Databases
{
    public static class AchievementDatabase
    {
        public static Dictionary<uint, string> Achievements;
        public static void Load()
        {
            var f = File.OpenText("resources/data/achievements.tsv");
            Achievements = new Dictionary<uint, string>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = UInt32.Parse(s[0]);
                var name = s[1];

                Achievements.Add(id, name);
            }
        }
    }
}
