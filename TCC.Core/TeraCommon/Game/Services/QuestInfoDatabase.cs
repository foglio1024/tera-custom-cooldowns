using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Tera.Game
{
    public class QuestInfoDatabase
    {
        private Dictionary<int,string> _lookup = new Dictionary<int, string>();
        public QuestInfoDatabase(string folder, string language)
        {
            var reader = new StreamReader(File.OpenRead(Path.Combine(folder, $"quests\\battle-{language}.tsv")));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var values = line.Split('\t');
                var id = int.Parse(values[0]);
                var name = values[1];
                _lookup[id] = name;
            }
            reader.Close();
            reader = new StreamReader(File.OpenRead(Path.Combine(folder, $"quests\\items-{language}.tsv")));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var values = line.Split('\t');
                var id = int.Parse(values[0]);
                var name = values[1];
                _lookup[id] = name;
            }
        }

        public string Get(int id)
        {
            return !_lookup.ContainsKey(id) ? id.ToString() : _lookup[id];
        }
    }

}