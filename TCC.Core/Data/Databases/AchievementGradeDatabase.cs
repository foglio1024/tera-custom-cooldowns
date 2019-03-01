using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class AchievementGradeDatabase : DatabaseBase
    {
        public Dictionary<uint, string> Grades { get; }

        protected override string FolderName => "achi_grade";

        protected override string Extension => "tsv";

        public AchievementGradeDatabase(string lang) : base(lang)
        {
            Grades = new Dictionary<uint, string>();
        }

        public override void Load()
        {
            Grades.Clear();
            var lines = File.ReadAllLines(FullPath);
            foreach (var line in lines)
            {
                //var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                if (!uint.TryParse(s[0], out var id)) continue;
                var name = s[1];

                Grades.Add(id, name);
            }
        }
    }
}
