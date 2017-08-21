using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public static class AchievementGradeDatabase
    {
        public static Dictionary<uint, string> Grades;
        public static void Load()
        {
            var f = File.OpenText("resources/data/achievement-grade-info.tsv");
            Grades = new Dictionary<uint, string>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = UInt32.Parse(s[0]);
                var name = s[1];

                Grades.Add(id, name);
            }
        }

    }
}
