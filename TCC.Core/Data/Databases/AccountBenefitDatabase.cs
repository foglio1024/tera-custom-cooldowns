using System;
using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{

    public class AccountBenefitDatabase : DatabaseBase
    {
        protected override string FolderName => "acc_benefits";
        protected override string Extension => "tsv";

        public Dictionary<uint, string> Benefits;

        public AccountBenefitDatabase(string lang) : base(lang)
        {
            Benefits = new Dictionary<uint, string>();
        }

        public override void Load()
        {
            Benefits.Clear();
            var f = File.OpenText(FullPath);
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) return;
                if (line == "") continue;
                var s = line.Split('\t');
                if (!uint.TryParse(s[0], out var val)) continue;
                Benefits.Add(val, s[1]);
            }
        }

    }

    public abstract class DatabaseBase
    {
        protected string _language;
        protected abstract string FolderName { get; }
        protected abstract string Extension { get; }

        protected string FullPath => Path.Combine(App.DataPath, $"{FolderName}/{FolderName}-{_language}.{Extension}");
        public bool Exists => File.Exists(FullPath);

        public abstract void Load();
        public DatabaseBase(string lang)
        {
            _language = lang;
        }
    }
}
