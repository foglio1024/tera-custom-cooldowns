using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class SocialDatabase
    {
        public Dictionary<uint, string> Social { get; }
        public SocialDatabase(string lang)
        {
            var f = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"resources/data/social/social-{lang}.tsv"));
            Social = new Dictionary<uint, string>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                var id = Convert.ToUInt32(s[0]);
                var phrase = s[1];

                Social.Add(id, phrase);
            }
        }
    }
}
