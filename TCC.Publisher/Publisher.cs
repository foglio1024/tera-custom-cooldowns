using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nostrum;
using Newtonsoft.Json.Linq;
using Octokit;
using SevenZip;
using TCC.Utils;

namespace TCC.Publisher;

public class Publisher
{
    static Publisher? _instance;
    public static Publisher Instance => _instance ??= new Publisher();

    readonly GitHubClient _client;

    readonly PublisherSettings _settings;

    /// <summary>
    /// TCC/release/
    /// </summary>
    readonly string _releaseFolder;
    /// <summary>
    /// TCC/release/TCC.exe
    /// </summary>
    string _exePath => Path.Combine(_releaseFolder, "TCC.dll");
    /// <summary>
    /// X.Y.Z
    /// </summary>
    string _stringVersion = ""; // "X.Y.Z"
    /// <summary>
    /// -b
    /// </summary>
    string _beta = "";  // "-b"
    /// <summary>
    /// TCC-X.Y.Z-b.zip
    /// </summary>
    string _zipName => $"TCC-{_stringVersion}{_beta}.zip";
    /// <summary>
    /// vX.Y.Z-b
    /// </summary>
    string _tag => $"v{_stringVersion}{_beta}";

    Publisher()
    {
        _settings = JsonConvert.DeserializeObject<PublisherSettings>(File.ReadAllText("tcc_publisher_settings.json"))!;
        _releaseFolder = Path.Combine(_settings.LocalRepositoryPath, "release");
        _client = new GitHubClient(new ProductHeaderValue("TCC.Publisher"))
        {
            Credentials = new Credentials(_settings.GithubToken)
        };
            
    }

    public static void Init()
    {
        _instance = new Publisher();
    }


    public string GetVersion()
    {
        Logger.WriteLine("    Getting version...");
        var an = AssemblyName.GetAssemblyName(_exePath);
        var v = an.Version;
        if (v == null)
        {
            Logger.WriteLine($"    Failed to get TCC version.");
            return "";
        }
        _stringVersion = $"{v.Major}.{v.Minor}.{v.Build}";
        // ReSharper disable once HeuristicUnreachableCode
        _beta = GlobalFlags.IsBeta ? "-b" : "";
        Logger.WriteLine($"    TCC version is {_stringVersion}{_beta}");
        Logger.WriteLine("-------------");
        return $"{_stringVersion}{_beta}";
    }

    public async Task CompressRelease()
    {
        // delete old release zip
        foreach (var f in Directory.GetFiles(_releaseFolder))
        {
            if (!f.EndsWith(".zip")) continue;
            Logger.WriteLine($"    Deleting {f}");
            File.Delete(f);
        }

        SevenZipBase.SetLibraryPath(_settings.SevenZipLibPath);
        Logger.WriteLine("    Starting compression...");
        await Task.Factory.StartNew(() =>
        {
            var files = new Dictionary<int, string?>();
            var comp = new SevenZipCompressor
            {
                CompressionLevel = CompressionLevel.Ultra,
                CompressionMethod = CompressionMethod.Deflate,
                CompressionMode = CompressionMode.Create,
                ArchiveFormat = OutArchiveFormat.Zip

            };
            comp.CompressDirectory(_releaseFolder, _zipName);

            var extr = new SevenZipExtractor(_zipName);
            foreach (var f in extr.ArchiveFileData)
            {
                if (_settings.ExcludedFiles.Any(ex => f.FileName.StartsWith(ex)))
                {
                    files[f.Index] = null;
                }
            }
            comp.ModifyArchive(_zipName, files);
        });
        Logger.Write(" Done\n");
        Logger.WriteLine("    Copying zip to release folder...");
        File.Move(_zipName, Path.Combine(_releaseFolder, _zipName));

        Logger.Write(" Done\n");

    }
    public void UpdateVersionCheckFile()
    {
        Logger.WriteLine("    Building version file...");
        var url = $"https://github.com/Foglio1024/Tera-custom-cooldowns/releases/download/v{ _stringVersion }{ _beta }/{_zipName}";
        var versionCheckFile = Path.Combine(_settings.LocalRepositoryPath, "version");
        var sb = new StringBuilder();
        sb.AppendLine(_stringVersion);
        Logger.WriteLine($"    Added version: {_stringVersion}.");
        sb.Append(url);
        Logger.WriteLine($"    Added URL: {url}.");
        File.WriteAllText(versionCheckFile, sb.ToString());
        Logger.WriteLine("    File saved.");
    }
    public async Task CreateRelease(string changelog)
    {
        try
        {
            await _client.Repository.Release.Get(_settings.RepositoryOwner, _settings.RepositoryName, _tag);
            Logger.WriteLine($"WARNING: Release already existing.");
        }
        catch (NotFoundException)
        {
            var newRelease = new NewRelease($"v{_stringVersion}{_beta}")
            {
                Name = $"v{_stringVersion}{_beta}",
                Body = changelog,
                Prerelease = false,
                TargetCommitish = string.IsNullOrEmpty(_beta) ? "master" : "beta"
            };
            await Task.Run(() => _client.Repository.Release.Create(_settings.RepositoryOwner, _settings.RepositoryName, newRelease));
            ExecuteWebhook(changelog);
            try
            {
                UpdateFirestoreVersion();
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Errors while updating Firestore version: {e}");
            }
            Logger.WriteLine($"Release created");
        }
    }

    void UpdateFirestoreVersion()
    {
        var msg = new JObject
        {
            {"version", $"{_stringVersion}{_beta}"},
            {"hash", HashUtils.GenerateFileHash(_exePath) }
        };

        using var c = MiscUtils.GetDefaultHttpClient();
        c.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");
        c.DefaultRequestHeaders.Add(HttpRequestHeader.AcceptCharset.ToString(), "utf-8");
        c.PostAsync(_settings.FirestoreUrl, new StringContent(msg.ToString(), Encoding.UTF8));
    }

    void ExecuteWebhook(string changelog)
    {
        var msg = new JObject
        {
            {"username", $"TCC v{_stringVersion}{_beta}" },
            {"content", $"{changelog}"},
            {"avatar_url", "https://i.imgur.com/jiWuveM.png" }
        };

        using var c = MiscUtils.GetDefaultHttpClient();
        c.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");
        c.PostAsync(_settings.DiscordWebhook, new StringContent(msg.ToString(), Encoding.UTF8));
    }

    public async Task Upload()
    {
        var rls = await _client.Repository.Release.Get(owner: _settings.RepositoryOwner, name: _settings.RepositoryName, tag: $"v{_stringVersion}{_beta}");
        if (rls.Assets.Any(x => x.Name == _zipName))
        {
            Logger.WriteLine("ERROR: This release already contains an asset with the same name.");
            return;
        }

        var str = new MemoryStream();
        var bytes = File.ReadAllBytes(Path.Combine(_releaseFolder, _zipName));
        str.Write(bytes, 0, bytes.Length);
        str.Seek(0, SeekOrigin.Begin);

        Logger.WriteLine($"Release zip loaded");

        var au = new ReleaseAssetUpload
        {
            FileName = _zipName,
            ContentType = "application/zip",
            RawData = str
        };

        Logger.WriteLine($"Uploading asset");
        await _client.Repository.Release.UploadAsset(rls, au);
        Logger.WriteLine($"Asset uploaded");
    }

}