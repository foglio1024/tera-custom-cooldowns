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
                if (string.IsNullOrEmpty(line)) return;
                var s = line.Split('\t');
                Benefits.Add(uint.Parse(s[0]), s[1]);
            }
        }
    }
}
