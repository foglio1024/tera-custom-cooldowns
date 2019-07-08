using FoglioUtils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TCC.Data;
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

        private static string DownloadedIconsDir => Path.Combine(App.BasePath, "tera-used-icons-master");

        private static readonly string DatabaseHashFileUrl = $"https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/{(App.Experimental ? "experimental" : "master")}/database-hashes.json";

        public static Dictionary<string, string> DatabaseHashes { get; set; }

        public static void CheckServersFile()
        {
            var path = Path.Combine(App.DataPath, "servers.txt");
            if (!File.Exists(path) || HashUtils.GenerateFileHash(path) != DatabaseHashes["servers.txt"])
                DownloadServersFile();
        }
        private static void DownloadServersFile()
        {
            if (!Directory.Exists(App.DataPath)) Directory.CreateDirectory(App.DataPath);
            using (var c = MiscUtils.GetDefaultWebClient())
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
            using (var c = MiscUtils.GetDefaultWebClient())
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

        public static bool IsExperimentalNewer()
        {
            try
            {
                using (var c = MiscUtils.GetDefaultWebClient())
                {
                    var st = c.OpenRead(AppVersionExperimentalUrl);
                    if (st == null) return false;
                    var newVersionInfo = new StreamReader(st).ReadLine();
                    if (newVersionInfo == null) return false;
                    if (Version.Parse(newVersionInfo) > Assembly.GetExecutingAssembly().GetName().Version) return true;
                }
            }
            catch (Exception e)
            {
                Log.F($"[IsExperimentalNewer] Failed to check experimental version {e}");
            }
            return false;
        }

        private static async Task DownloadIcons()
        {
            using (var c = MiscUtils.GetDefaultWebClient())
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
                    await Task.Factory.StartNew(() => c.DownloadFileAsync(new Uri(IconsUrl), "icons.zip"));
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
                    try
                    {
                        File.Copy(newPath, newPath.Replace(DownloadedIconsDir, "resources/images"), true);
                    }
                    catch
                    {
                        Log.F("Failed to copy icon " + newPath);
                    }
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
            try
            {
                var url = $"https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/{relativePath.Replace("\\", "/")}";
                var destPath = Path.Combine(App.DataPath, relativePath);
                var destDir = Path.GetDirectoryName(destPath);
                if (!Directory.Exists(destDir) && destDir != null) Directory.CreateDirectory(destDir);
                using (var c = MiscUtils.GetDefaultWebClient())
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
            using (var c = MiscUtils.GetDefaultWebClient())
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
                    File.WriteAllText(Path.Combine(App.BasePath, "update-check-error.txt"), $"{ex.Message}\n{ex.StackTrace}\n{ex.Source}\n{ex}\n{ex.Data}\n{ex.InnerException}\n{ex.TargetSite}");
                }
            }
        }

        public async static void ForceUpdateExperimental()
        {
            using (var c = MiscUtils.GetDefaultWebClient())
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

        public async static Task CheckAppVersion()
        {
            try
            {
                var vp = new VersionParser();
                if (!vp.Valid) return;
                if (!vp.IsNewer) return;
                if (!App.SplashScreen.AskUpdate($"TCC v{vp.NewVersionNumber} available. Download now?")) return;

                await Update(vp.NewVersionUrl);
            }
            catch (Exception e)
            {
                Log.F($"Error while checking update. \nException:\n{e.Message}\n{e.StackTrace}");
                if (!App.SplashScreen.AskUpdate("Error while checking updates. Try again?")) return;
                await CheckAppVersion();
            }
        }
        private static bool _waitingDownload = true;
        private async static Task Update(string url)
        {
            using (var c = MiscUtils.GetDefaultWebClient())
            {
                try
                {
                    App.SplashScreen.SetText("Downloading update...");
                    c.DownloadFileCompleted += (s, ev) => _waitingDownload = false;
                    c.DownloadProgressChanged += App.SplashScreen.UpdateProgress;
                    await App.SplashScreen.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        c.DownloadFileAsync(new Uri(url), "update.zip");
                    }));

                    while (_waitingDownload) Thread.Sleep(1000); //only way to wait for downlaod

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
                    var res = TccMessageBox.Show("Error while downloading update. Try again? If the error perists download TCC manually.", MessageBoxType.ConfirmationWithYesNo);
                    if (res != System.Windows.MessageBoxResult.Yes) return;
                    await Update(url);
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
            using (var c = MiscUtils.GetDefaultWebClient())
            {
                var f = c.OpenRead(DatabaseHashFileUrl);
                if (f == null) return;
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

        public static void TryDeleteUpdater()
        {
            try
            {
                File.Delete(Path.Combine(App.BasePath, "TCCupdater.exe"));
            }
            catch
            {
                /* ignored*/
            }
        }


        private class VersionParser
        {
            public string NewVersionNumber { get; }
            public string NewVersionUrl { get; }
            public Version Version => Version.Parse(NewVersionNumber);
            public bool IsNewer => Version > Assembly.GetExecutingAssembly().GetName().Version;
            public bool Valid { get; }

            public VersionParser(bool forceExperimental = false)
            {
                using (var c = MiscUtils.GetDefaultWebClient())
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
