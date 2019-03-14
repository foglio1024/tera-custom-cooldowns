using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using SevenZip;
using Application = System.Windows.Application;

namespace TCC.Publisher
{
    public class Publisher
    {
        static GitHubClient _client;
        static GitHubClient Client => _client ?? (_client = new GitHubClient(new ProductHeaderValue("TCC.Publisher"))
        {
            Credentials = new Credentials(File.ReadAllText("D:/Repos/TCC/TCC.Publisher/github-token.txt"))
        });

        const string TCCPath = "D:/Repos/TCC";
        const string Repo = "Tera-custom-cooldowns";
        const string Owner = "Foglio1024";

        static string ReleaseFolder => Path.Combine(TCCPath, "release");
        static string _stringVersion = ""; // "X.Y.Z"
        static string _experimental = "";  // "-e"
        static string ZipName => $"TCC-{_stringVersion}{_experimental}.zip";
        static string Tag => $"v{_stringVersion}{_experimental}";

        public static void GetVersion()
        {
            Logger.WriteLine("    Getting version...");
            var an = AssemblyName.GetAssemblyName("D:/Repos/TCC/release/TCC.exe");
            var v = an.Version;
            _stringVersion = $"{v.Major}.{v.Minor}.{v.Build}";
            _experimental = TCC.App.Experimental ? "-e" : "";
            Logger.WriteLine($"    TCC version is {_stringVersion}{_experimental}");
            Logger.WriteLine("-------------");
        }
        public static async void Generate()
        {

            Logger.WriteLine("    Compressing release...");
            await CompressRelease();
            Logger.WriteLine("    Release compressed.");
            Logger.WriteLine("-------------");

            Logger.WriteLine("    Updating version check file...");
            UpdateVersionCheckFile();
            Logger.WriteLine("    Version check file updated.");
            Logger.WriteLine("-------------");
        }
        private static async Task CompressRelease()
        {
            foreach (var f in Directory.GetFiles(ReleaseFolder))
            {
                if (f.EndsWith(".zip"))
                {
                    Logger.WriteLine($"    Deleting {f}");
                    File.Delete(f);
                }
                else
                {
                    App.Exclusions.ForEach(e =>
                    {
                        if (f.Contains(e))
                        {
                            Logger.WriteLine($"    Deleting {f}");
                            File.Delete(f);
                        }
                    });
                }
            }

            SevenZipBase.SetLibraryPath("C:/Program Files/7-Zip/7z.dll");
            Logger.WriteLine("    Starting compression...");
            await Task.Factory.StartNew(() => new SevenZipCompressor
            {
                CompressionLevel = CompressionLevel.Ultra,
                CompressionMethod = CompressionMethod.Deflate,
                CompressionMode = CompressionMode.Create,
                ArchiveFormat = OutArchiveFormat.Zip
            }.CompressDirectory(ReleaseFolder, ZipName));
            Logger.Write(" Done\n");
            Logger.WriteLine("    Copying zip to release folder...");
            File.Move(ZipName, Path.Combine(ReleaseFolder, ZipName));

            Logger.Write(" Done\n");

        }
        private static void UpdateVersionCheckFile()
        {
            Logger.WriteLine("    Building version file...");
            var url = $"https://github.com/Foglio1024/Tera-custom-cooldowns/releases/download/v{_stringVersion}{_experimental}/{ZipName}";
            var versionCheckFile = Path.Combine(TCCPath, "version");
            var sb = new StringBuilder();
            sb.AppendLine(_stringVersion);
            Logger.WriteLine($"    Added version: {_stringVersion}.");
            sb.Append(url);
            Logger.WriteLine($"    Added URL: {url}.");
            File.WriteAllText(versionCheckFile, sb.ToString());
            Logger.WriteLine("    File saved.");
        }
        public static async Task CreateRelease()
        {
            try
            {
                await Client.Repository.Release.Get(Owner, Repo, Tag);
                Logger.WriteLine($"WARNING: Release already existing.");
            }
            catch (NotFoundException)
            {
                var newRelease = new NewRelease($"v{_stringVersion}{_experimental}")
                {
                    Name = $"v{_stringVersion}{_experimental}",
                    Body = (Application.Current.MainWindow as MainWindow)?.ReleaseNotesTb.Text, // from UI
                    Prerelease = false,
                    TargetCommitish = string.IsNullOrEmpty(_experimental) ? "master" : "experimental"
                };
                await Task.Run(() => Client.Repository.Release.Create(Owner, Repo, newRelease));
                Logger.WriteLine($"Release created");
            }
        }
        public static async Task Upload()
        {
            var rls = await Client.Repository.Release.Get(owner: Owner, name: Repo, tag: $"v{_stringVersion}{_experimental}");
            if (rls.Assets.Any(x => x.Name == ZipName))
            {
                Logger.WriteLine("ERROR: This release already contains an asset with the same name.");
                return;
            }

            var str = new MemoryStream();
            var bytes = File.ReadAllBytes(Path.Combine(ReleaseFolder, ZipName));
            str.Write(bytes, 0, bytes.Length);
            str.Seek(0, SeekOrigin.Begin);

            Logger.WriteLine($"Release zip loaded");

            var au = new ReleaseAssetUpload
            {
                FileName = ZipName,
                ContentType = "application/zip",
                RawData = str
            };

            Logger.WriteLine($"Uploading asset");
            await Client.Repository.Release.UploadAsset(rls, au);
            Logger.WriteLine($"Asset uploaded");


        }
    }
}