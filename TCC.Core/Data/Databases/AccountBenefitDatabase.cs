using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class AccountBenefitDatabase
    {
        public Dictionary<uint, string> Benefits;

        public AccountBenefitDatabase(string lang)
        {
            Load(lang);
        }
        public void Load(string lang)
        {
            Benefits = new Dictionary<uint, string>();
            var f = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + $"/resources/data/acc_benefits/acc_benefits-{lang}.tsv");
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) return;
                if(line == "") continue;
                var s = line.Split('\t');
                if (!uint.TryParse(s[0], out var val)) continue;
                Benefits.Add(val, s[1]);
            }
        }
    }
}
