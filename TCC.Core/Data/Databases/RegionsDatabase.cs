using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class RegionsDatabase : DatabaseBase
    {
        protected override string FolderName => "regions";
        protected override string Extension => "tsv";

        private Dictionary<uint, string> Names { get; }

        public RegionsDatabase(string lang) : base(lang)
        {
            Names = new Dictionary<uint, string>();

        }
        public override void Load()
        {
            Names.Clear();
            //var f = File.OpenText(FullPath);
            var lines = File.ReadAllLines(FullPath);
            foreach (var line in lines)
            {
                //var line = f.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;

                var s = line.Split('\t');

                var id = Convert.ToUInt32(s[0]);
                var name = s[1];

                Names.Add(id, name);
            }
        }
        public string GetZoneName(uint zoneId)
        {
            return Names.TryGetValue(zoneId, out var name) ? name : $"Unknown ({zoneId})";
        }

    }
}
