using System.IO;
using FoglioUtils;
using TCC.Utilities;

namespace TCC.Data.Databases
{
    public abstract class DatabaseBase
    {
        protected string Language;
        protected abstract string FolderName { get; }
        protected abstract string Extension { get; }

        public string RelativePath => $"{FolderName}/{FolderName}-{Language}.{Extension}";
        protected string FullPath => Path.Combine(App.DataPath, RelativePath);
        public virtual bool Exists => File.Exists(FullPath);
        public bool IsUpToDate => outdatedCount == 0 && Exists;
        protected int outdatedCount;

        public abstract void Load();
        public virtual void CheckVersion(string customAbsPath = null, string customRelPath = null)
        {
            if (!Exists) return;
            var localHash = HashUtils.GenerateFileHash(customAbsPath ?? FullPath);
            if (UpdateManager.DatabaseHashes.Count == 0) return;
            if (!UpdateManager.DatabaseHashes.TryGetValue(customRelPath ?? RelativePath, out var remoteHash)) return;
            if (remoteHash == localHash) return;
            Log.CW($"Hash mismatch for {customRelPath ?? RelativePath}");
            outdatedCount++;

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