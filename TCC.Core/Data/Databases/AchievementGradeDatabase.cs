using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class AchievementGradeDatabase
    {
        public Dictionary<uint, string> Grades { get; }
        public AchievementGradeDatabase(string lang)
        {
            var f = File.OpenText($"resources/data/achi_grade/achi_grade-{lang}.tsv");
            Grades = new Dictionary<uint, string>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                if (!uint.TryParse(s[0], out var id)) continue;
                var name = s[1];

                Grades.Add(id, name);
            }
        }

    }
}
