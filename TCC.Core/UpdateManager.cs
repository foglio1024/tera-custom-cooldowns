using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Data.Databases;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{

    public static class UpdateManager
    {
        private static System.Timers.Timer _checkTimer;

        private const string IconsUrl = "https://github.com/Foglio1024/tera-used-icons/archive/master.zip";
        private const string IconsVersionUrl = "https://raw.githubusercontent.com/Foglio1024/tera-used-icons/master/current_version";

        private const string AppVersionUrl = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/master/version";
        private const string AppVersionExperimentalUrl = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/experimental/version";

        private const string DownloadedIconsDir = "tera-used-icons-master";

        public const string DatabaseHashFileUrl = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/experimental/database-hashes.json";

        public static Dictionary<string, string> DatabaseHashes { get; set; }

        public static void CheckServersFile()
        {
            var path = Path.Combine(App.DataPath, "servers.txt");
            if (!File.Exists(path) || Utils.GenerateFileHash(path) != DatabaseHashes["servers.txt"])
                DownloadServersFile();
        }
        private static void DownloadServersFile()
        {
            if (!Directory.Exists(App.DataPath)) Directory.CreateDirectory(App.DataPath);
            using (var c = Utils.GetDefaultWebClient())
            {

                try
                {

                    c.DownloadFile("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/servers.txt", Path.Combine(App.DataPath, "servers.txt"));
                }
                catch
                {
                    var res = TccMessageBox.Show("Failed to download servers file. Try again?", MessageBoxType.ConfirmationWithYesNo);
                    if (res == System.Windows.MessageBoxResult.Yes) DownloadServersFile();

                }
            }
        }

        public static async Task CheckIconsVersion()
        {
            using (var c = Utils.GetDefaultWebClient())
            {
                try
                {
                    var st = c.OpenRead(IconsVersionUrl);
                    if (st == null) return;
                    var newVersion = Convert.ToInt32(new StreamReader(st).ReadLine());
                    var currentVersion = 0;
                    var currVersionFilePath = Path.Combine(App.ResourcesPath, "images/current_version");
                    if (File.Exists(currVersionFilePath))
                    {
                        using (var str = File.OpenText(currVersionFilePath))
                        {
                            currentVersion = Convert.ToInt32(str.ReadLine());
                            str.Close();
                        }
                    }

                    if (newVersion <= currentVersion) return;
                    if (!App.SplashScreen.AskUpdate($"Icon database v{newVersion} available. Download now?")) return;

                    await DownloadIcons();
                }
                catch (Exception)
                {
                    if (!App.SplashScreen.AskUpdate("Error while checking icon database update. Try again?")) return;
                    await CheckIconsVersion();
                }
            }
        }

        public static void CheckDatabaseHash()
        {
            DatabaseHashes = new Dictionary<string, string>();

            try
            {
                DownloadDatabaseHashes();
            }
            catch (Exception ex)
            {
                Log.F($"Failed to download database hashes. nException: {ex.Message}\n{ex.StackTrace}");
                if (App.SplashScreen.AskUpdate("Failed to download database hashes. Try again?")) CheckDatabaseHash();
            }
        }

        private static async Task DownloadIcons()
        {

            using (var c = Utils.GetDefaultWebClient())
            {
                //c.DownloadProgressChanged += App.SplashScreen.UpdateProgress;
                c.DownloadFileCompleted += async (_, args) =>
                {
                    if (args.Error != null)
                    {
                        var res = TccMessageBox.Show("Failed to download icons, try again?", MessageBoxType.ConfirmationWithYesNo);
                        if (res == System.Windows.MessageBoxResult.Yes) await DownloadIcons();
                    }
                    else
                    {
                        if (!App.Loading) WindowManager.FloatingButton.NotifyExtended("TCC update manager", "Done downloading icons.", NotificationType.Success, 2000);
                        ExtractIcons();
                    }
                };
                try
                {
                    App.SplashScreen.SetText("Downloading icons...");
                    await Task.Factory.StartNew(() => c.DownloadFileAsync(new Uri(IconsUrl), "icons.zip")); //not awaited
                }
                catch (Exception)
                {
                    if (!App.SplashScreen.AskUpdate("Error while downloading database. Try again?")) return;
                    await DownloadIcons();
                }
            }
        }

        private static void ExtractIcons()
        {
            try
            {
                if (Directory.Exists(DownloadedIconsDir)) Directory.Delete(DownloadedIconsDir, true);

                //App.SplashScreen.SetText("Extracting database...");

                if (!App.Loading) WindowManager.FloatingButton.NotifyExtended("TCC update manager", "Extracting icons...", NotificationType.Success, 2000);
                ZipFile.ExtractToDirectory("icons.zip", App.BasePath);
                //App.SplashScreen.SetText("Extracting database... Done.");

                //App.SplashScreen.SetText("Creating directories...");
                Directory.GetDirectories(DownloadedIconsDir, "*", SearchOption.AllDirectories).ToList().ForEach(dirPath =>
                {
                    Directory.CreateDirectory(dirPath.Replace(DownloadedIconsDir, "resources/images"));
                });
                //App.SplashScreen.SetText("Creating directories... Done.");

                //App.SplashScreen.SetText("Copying files...");
                Directory.GetFiles(DownloadedIconsDir, "*.*", SearchOption.AllDirectories).ToList().ForEach(newPath =>
                {
                    File.Copy(newPath, newPath.Replace(DownloadedIconsDir, "resources/images"), true);
                });
                //App.SplashScreen.SetText("Copying files... Done.");

                CleanTempIcons();
                if (!App.Loading) WindowManager.FloatingButton.NotifyExtended("TCC update manager", "Icons updated successfully", NotificationType.Success, 2000);

                //App.SplashScreen.SetText("Icons updated successfully.");

            }
            catch
            {
                var res = TccMessageBox.Show("Error while extracting icons. Try again?", MessageBoxType.ConfirmationWithYesNo);
                if (res == System.Windows.MessageBoxResult.Yes) ExtractIcons();
            }
        }

        public static void UpdateDatabase(string relativePath)
        {
            // example https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/acc_benefits/acc_benefits-EU-EN.tsv
            var url = $"https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/{relativePath.Replace("\\", "/")}";
            var destPath = Path.Combine(App.DataPath, relativePath);
            if (!Directory.Exists(Path.GetDirectoryName(destPath))) Directory.CreateDirectory(Path.GetDirectoryName(destPath));

            try
            {
                using (var c = Utils.GetDefaultWebClient())
                {
                    c.DownloadFile(url, destPath);
                }
            }
            catch
            {
                var res = TccMessageBox.Show($"Failed to download database file {Path.GetFileNameWithoutExtension(relativePath)}. Try again?", MessageBoxType.ConfirmationWithYesNo);
                if (res == System.Windows.MessageBoxResult.Yes) UpdateDatabase(relativePath);
            }
        }

        private static void CleanTempIcons()
        {
            try
            {
                Directory.Delete(DownloadedIconsDir, true);
                File.Delete("icons.zip");
            }
            catch { }
        }

        public static void StartPeriodicCheck()
        {
            _checkTimer = new System.Timers.Timer(60 * 10 * 1000);
            _checkTimer.Elapsed += CheckTimer_Elapsed;
            _checkTimer.Start();
        }

        private static void CheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _checkTimer.Stop();
            CheckAppVersionPeriodic();
            _checkTimer.Start();
        }

        private static void CheckAppVersionPeriodic()
        {
            using (var c = Utils.GetDefaultWebClient())
            {
                try
                {
                    var st = c.OpenRead(App.Experimental ? AppVersionExperimentalUrl : AppVersionUrl);
                    if (st == null) return;
                    var newVersionInfo = new StreamReader(st).ReadLine();
                    if (newVersionInfo == null) return;
                    if (Version.Parse(newVersionInfo) <= Assembly.GetExecutingAssembly().GetName().Version) return;

                    ChatWindowManager.Instance.AddTccMessage($"TCC v{newVersionInfo} available!");
                    WindowManager.FloatingButton.NotifyExtended("Update manager", $"TCC v{newVersionInfo} available!", NotificationType.Success);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Path.Combine(App.BasePath, "update-check-error.txt"), ex.Message + "\r\n" +
                             ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                             "\r\n" + ex.TargetSite);
                }
            }
        }

        public async static void ForceUpdateExperimental()
        {
            using (var c = Utils.GetDefaultWebClient())
            {
                try
                {
                    var vp = new VersionParser(forceExperimental: true);
                    if (!vp.Valid) return;

                    WindowManager.FloatingButton.NotifyExtended("TCC update manager", "Download started", NotificationType.Success, 3000);
                    c.DownloadFile(new Uri(vp.NewVersionUrl), "update.zip");

                    WindowManager.FloatingButton.NotifyExtended("TCC update manager", "Extracting zip", NotificationType.Success, 3000);
                    if (Directory.Exists(Path.Combine(App.BasePath, "tmp"))) Directory.Delete(Path.Combine(App.BasePath, "tmp"), true);
                    ZipFile.ExtractToDirectory("update.zip", Path.Combine(App.BasePath, "tmp"));

                    WindowManager.FloatingButton.NotifyExtended("TCC update manager", "Moving files", NotificationType.Success, 2000);
                    File.Move(Path.Combine(App.BasePath, "tmp/TCCupdater.exe"), Path.Combine(App.BasePath, "TCCupdater.exe"));

                    WindowManager.FloatingButton.NotifyExtended("TCC update manager", "Starting updater", NotificationType.Success, 1000);
                    await Task.Delay(1000).ContinueWith(t => Process.Start(Path.GetDirectoryName(typeof(App).Assembly.Location) + "/TCCupdater.exe", "update"));
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Log.F($"Error while checking updates. \nException: {ex.Message}\n{ex.StackTrace}");
                    if (TccMessageBox.Show("Error while checking updates. Try again?", MessageBoxType.ConfirmationWithYesNo) != System.Windows.MessageBoxResult.Yes) return;
                    ForceUpdateExperimental();
                }
            }
        }

        public static void CheckAppVersion()
        {
            try
            {
                var vp = new VersionParser();
                if (!vp.Valid) return;
                if (!vp.IsNewer) return;
                if (!App.SplashScreen.AskUpdate($"TCC v{vp.NewVersionNumber} available. Download now?")) return;

                Update(vp.NewVersionUrl);
            }
            catch (Exception e)
            {
                Log.F($"Error while checking update. \nException:\n{e.Message}\n{e.StackTrace}");
                if (!App.SplashScreen.AskUpdate("Error while checking updates. Try again?")) return;
                CheckAppVersion();
            }
        }

        private async static void Update(string url)
        {
            using (var c = Utils.GetDefaultWebClient())
            {
                try
                {
                    //var ready = false;
                    //c.DownloadFileCompleted += (s, ev) => ready = true;
                    App.SplashScreen.SetText("Downloading update...");
                    c.DownloadProgressChanged += App.SplashScreen.UpdateProgress;
                    await App.SplashScreen.Dispatcher.BeginInvoke(new Action(() => c.DownloadFileAsync(new Uri(url), "update.zip")));
                    //while (!ready) Thread.Sleep(1);

                    App.SplashScreen.SetText("Extracting zip...");
                    if (Directory.Exists(Path.Combine(App.BasePath, "tmp"))) Directory.Delete(Path.Combine(App.BasePath, "tmp"), true);
                    ZipFile.ExtractToDirectory("update.zip", Path.Combine(App.BasePath, "tmp"));

                    App.SplashScreen.SetText("Moving files...");
                    File.Move(Path.Combine(App.BasePath, "tmp/TCCupdater.exe"), Path.Combine(App.BasePath, "TCCupdater.exe"));

                    App.SplashScreen.SetText("Starting updater...");
                    Process.Start(Path.GetDirectoryName(typeof(App).Assembly.Location) + "/TCCupdater.exe", "update");
                    Environment.Exit(0);
                }
                catch (Exception e)
                {
                    Log.F($"Error while downloading update. \nException:\n{e.Message}\n{e.StackTrace}");
                    if (!App.SplashScreen.AskUpdate("Error while downloading update. Try again? If the error perists download TCC manually.")) return;
                    Update(url);
                }
            }
        }

        public static void StopTimer()
        {
            _checkTimer?.Stop();
        }

        public static void DownloadDatabaseHashes()
        {
            DatabaseHashes.Clear();
            using (var c = Utils.GetDefaultWebClient())
            {
                var f = c.OpenRead(DatabaseHashFileUrl);
                using (var sr = new StreamReader(f))
                {
                    var sHashes = sr.ReadToEnd();
                    var jHashes = JObject.Parse(sHashes);
                    jHashes.Descendants().ToList().ForEach(jDesc =>
                    {
                        if (!(jDesc is JProperty jProp)) return;
                        DatabaseHashes[jProp.Name] = jProp.Value.ToString();
                    });
                }
            }
        }

        private class VersionParser
        {
            public string NewVersionNumber { get; }
            public string NewVersionUrl { get; }
            public Version Version => Version.Parse(NewVersionNumber);
            public bool IsNewer => Version > Assembly.GetExecutingAssembly().GetName().Version;
            public bool Valid { get; } = false;

            public VersionParser(bool forceExperimental = false)
            {
                using (var c = Utils.GetDefaultWebClient())
                {
                    var st = c.OpenRead(App.Experimental || forceExperimental ? AppVersionExperimentalUrl : AppVersionUrl);
                    if (st == null) return;

                    using (var sr = new StreamReader(st))
                    {
                        NewVersionNumber = sr.ReadLine();
                        NewVersionUrl = sr.ReadLine();
                        Valid = true;
                    }
                }
            }
        }

    }
}
