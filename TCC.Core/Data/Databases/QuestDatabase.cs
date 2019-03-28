using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class QuestDatabase : DatabaseBase
    {
        public Dictionary<uint, string> Quests { get; }

        protected override string FolderName => "quests";
        protected override string Extension => "tsv";

        public QuestDatabase(string lang) : base(lang)
        {
            Quests = new Dictionary<uint, string>();
        }

        public override void Load()
        {
            Quests.Clear();
            //var f = File.OpenText(FullPath);
            var lines = File.ReadAllLines(FullPath);
            foreach (var line in lines)
            {
                //var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                if (!uint.TryParse(s[0], out var id)) continue;
                var name = s[1];

                Quests[id] =  name;
            }
        }
    }
}
