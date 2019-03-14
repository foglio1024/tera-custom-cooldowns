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
            var lines = File.ReadAllLines(FullPath);
            //var f = File.OpenText(FullPath);
            foreach (var line in lines)
            {
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
        protected string Language;
        protected abstract string FolderName { get; }
        protected abstract string Extension { get; }

        public string RelativePath => $"{FolderName}/{FolderName}-{Language}.{Extension}";
        protected string FullPath => Path.Combine(App.DataPath, RelativePath);
        public virtual bool Exists => File.Exists(FullPath);
        public bool IsUpToDate { get; private set; }


        public abstract void Load();
        public virtual void CheckVersion(string customAbsPath = null, string customRelPath = null)
        {
            if (!Exists) return;
            var localHash = Utils.GenerateFileHash(customAbsPath ?? FullPath);
            if (UpdateManager.DatabaseHashes.Count == 0) return;
            if (!UpdateManager.DatabaseHashes.ContainsKey(customRelPath ?? RelativePath)) return;
            if (UpdateManager.DatabaseHashes[customRelPath ?? RelativePath] != localHash)
            {
                Log.CW($"Hash mismatch for {customRelPath ?? RelativePath}");
                return;
            }

            IsUpToDate = true;

        }
        public DatabaseBase(string lang)
        {
            Language = lang;
        }

        public virtual void Update(string custom = null)
        {
            UpdateManager.UpdateDatabase(custom ?? RelativePath);
        }
    }
}
