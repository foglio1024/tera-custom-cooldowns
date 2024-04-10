using Nostrum;
using System.IO;
using TCC.Update;
using TCC.Utils;

namespace TCC.Data.Databases;

public abstract class DatabaseBase
{
    private int _outdatedCount;

    protected readonly string Language;

    protected abstract string FolderName { get; }
    protected abstract string Extension { get; }
    protected string RelativePath => $"{FolderName}/{FolderName}-{Language}.{Extension}";
    protected string FullPath { get; }
    public virtual bool Exists => File.Exists(FullPath);
    public bool IsUpToDate => _outdatedCount == 0 && Exists;

    protected DatabaseBase(string lang)
    {
        Language = lang;
        FullPath = Path.Combine(App.DataPath, RelativePath);
    }

    public abstract void Load();

    public virtual void CheckVersion(string customAbsPath = "", string customRelPath = "")
    {
        if (!Exists)
        {
            Log.F($"{(string.IsNullOrEmpty(customAbsPath) ? FullPath : customAbsPath)} not found. Skipping hash check.");
            return;
        }

        var localHash = HashUtils.GenerateFileHash(string.IsNullOrEmpty(customAbsPath) ? FullPath : customAbsPath);
        if (UpdateManager.DatabaseHashes.Count == 0)
        {
            Log.F("No database hashes in update manager. Skipping hash check.");
            return;
        }

        if (!UpdateManager.DatabaseHashes.TryGetValue(string.IsNullOrEmpty(customRelPath) ? RelativePath : customRelPath, out var remoteHash))
        {
            Log.F($"No entry found in update manager for {(string.IsNullOrEmpty(customAbsPath) ? FullPath : customAbsPath)}. Skipping hash check.");
            return;
        }

        if (remoteHash == localHash)
        {
            return;
        }

        _outdatedCount++;
    }

    public virtual void Update(string custom = "")
    {
        UpdateFromRemote(custom);
    }

    private void UpdateFromRemote(string custom)
    {
        UpdateManager.UpdateDatabase(string.IsNullOrEmpty(custom) ? RelativePath : custom);
    }
}