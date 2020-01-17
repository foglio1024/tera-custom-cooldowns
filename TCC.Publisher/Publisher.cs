using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FoglioUtils;
using Newtonsoft.Json.Linq;
using Octokit;
using SevenZip;
using Application = System.Windows.Application;

namespace TCC.Publisher
{
    public static class Publisher
    {
        private static GitHubClient _client;
        private static List<string> _exclusions;

        private const string TCC_PATH = "D:/Repos/TCC";
        private const string REPO = "Tera-custom-cooldowns";
        private const string OWNER = "Foglio1024";
        private const string SEVENZIP_LIB_PATH = "C:/Program Files/7-Zip/7z.dll";
        private const string TOKEN_PATH = "D:/Repos/TCC/TCC.Publisher/github-token.txt";
        private const string FIRESTORE_URL_PATH = "D:/Repos/TCC/TCC.Publisher/firestore_version_update.txt";
        private const string WEBHOOK_URL_PATH = "D:/Repos/TCC/TCC.Publisher/webhook.txt";
        private const string EXCLUSIONS_PATH = "D:/Repos/TCC/TCC.Publisher/exclusions.txt";

        /// <summary>
        /// TCC/release/
        /// </summary>
        private static readonly string _releaseFolder = Path.Combine(TCC_PATH, "release");
        /// <summary>
        /// TCC/release/TCC.exe
        /// </summary>
        private static string _exePath => Path.Combine(_releaseFolder, "TCC.exe");
        /// <summary>
        /// X.Y.Z
        /// </summary>
        private static string _stringVersion = ""; // "X.Y.Z"
        /// <summary>
        /// -e
        /// </summary>
        private static string _experimental = "";  // "-e"
        /// <summary>
        /// TCC-X.Y.Z-e.zip
        /// </summary>
        private static string _zipName => $"TCC-{_stringVersion}{_experimental}.zip";
        /// <summary>
        /// vX.Y.Z-e
        /// </summary>
        private static string _tag => $"v{_stringVersion}{_experimental}";

        public static void Init()
        {
            _exclusions = File.ReadAllLines(EXCLUSIONS_PATH).ToList();
            _client = new GitHubClient(new ProductHeaderValue("TCC.Publisher"))
            {
                Credentials = new Credentials(File.ReadAllText(TOKEN_PATH))
            };
        }


        public static string GetVersion()
        {
            Logger.WriteLine("    Getting version...");
            var an = AssemblyName.GetAssemblyName(_exePath);
            var v = an.Version;
            _stringVersion = $"{v.Major}.{v.Minor}.{v.Build}";
            _experimental = TCC.App.Beta ? "-e" : "";
            Logger.WriteLine($"    TCC version is {_stringVersion}{_experimental}");
            Logger.WriteLine("-------------");
            return $"{_stringVersion}{_experimental}";
        }

        public static async Task CompressRelease()
        {
            // delete old release zip
            foreach (var f in Directory.GetFiles(_releaseFolder))
            {
                if (!f.EndsWith(".zip")) continue;
                Logger.WriteLine($"    Deleting {f}");
                File.Delete(f);
            }

            SevenZipBase.SetLibraryPath(SEVENZIP_LIB_PATH);
            Logger.WriteLine("    Starting compression...");
            await Task.Factory.StartNew(() =>
            {
                var files = new Dictionary<int, string>();
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
                    if (_exclusions.Any(ex => f.FileName.StartsWith(ex)))
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
        public static void UpdateVersionCheckFile()
        {
            Logger.WriteLine("    Building version file...");
            var url = $"https://github.com/Foglio1024/Tera-custom-cooldowns/releases/download/v{_stringVersion}{_experimental}/{_zipName}";
            var versionCheckFile = Path.Combine(TCC_PATH, "version");
            var sb = new StringBuilder();
            sb.AppendLine(_stringVersion);
            Logger.WriteLine($"    Added version: {_stringVersion}.");
            sb.Append(url);
            Logger.WriteLine($"    Added URL: {url}.");
            File.WriteAllText(versionCheckFile, sb.ToString());
            Logger.WriteLine("    File saved.");
        }
        public static async Task CreateRelease(string changelog)
        {
            try
            {
                await _client.Repository.Release.Get(OWNER, REPO, _tag);
                Logger.WriteLine($"WARNING: Release already existing.");
            }
            catch (NotFoundException)
            {
                var newRelease = new NewRelease($"v{_stringVersion}{_experimental}")
                {
                    Name = $"v{_stringVersion}{_experimental}",
                    Body = changelog,
                    Prerelease = false,
                    TargetCommitish = string.IsNullOrEmpty(_experimental) ? "master" : "experimental"
                };
                await Task.Run(() => _client.Repository.Release.Create(OWNER, REPO, newRelease));
                ExecuteWebhook(changelog);
                UpdateFirestoreVersion();
                Logger.WriteLine($"Release created");
            }
        }

        private static void UpdateFirestoreVersion()
        {
            var msg = new JObject
            {
                {"version", $"{_stringVersion}{_experimental}"},
                {"hash", HashUtils.GenerateFileHash(_exePath) }
            };

            using (var c = MiscUtils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                c.UploadString(File.ReadAllText(FIRESTORE_URL_PATH), msg.ToString());
            }
        }

        private static void ExecuteWebhook(string changelog)
        {
            var msg = new JObject
            {
                {"username", $"TCC v{_stringVersion}{_experimental}" },
                {"content", $"{changelog}"},
                {"avatar_url", "https://i.imgur.com/jiWuveM.png" }
            };

            using (var client = MiscUtils.GetDefaultWebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.UploadString(File.ReadAllText(WEBHOOK_URL_PATH), msg.ToString());
            }
        }

        public static async Task Upload()
        {
            var rls = await _client.Repository.Release.Get(owner: OWNER, name: REPO, tag: $"v{_stringVersion}{_experimental}");
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
}