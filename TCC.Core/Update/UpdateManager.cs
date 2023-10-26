using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Newtonsoft.Json.Linq;
using Nostrum;
using TCC.Data;
using TCC.UI.Windows;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels;
using Timer = System.Timers.Timer;

namespace TCC.Update;

public static class UpdateManager
{
    const string AppVersionUrl = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/master/version";
    const string AppVersionBetaUrl = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/beta/version";
    static readonly string DatabaseHashFileUrl = $"https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/{(App.Beta ? "beta" : "master")}/database-hashes.json";

    static readonly Timer _checkTimer = new((App.ToolboxMode ? 2 : 10) * 60 * 1000);
    static bool _waitingDownload = true;

    public static Dictionary<string, string> DatabaseHashes { get; } = new();
    public static bool UpdateAvailable { get; private set; }
    public static async Task CheckAppVersion()
    {
        try
        {
            var vp = new VersionParser();
            if (!vp.Valid || !vp.IsNewer) return;
            if (!App.SplashScreen.VM.AskUpdate($"TCC v{vp.NewVersionNumber} available. Download now?")) return;

            await Update(vp.NewVersionUrl);
        }
        catch (Exception e)
        {
            Log.F($"Error while checking update. \nException:\n{e.Message}\n{e.StackTrace}");
            if (!App.SplashScreen.VM.AskUpdate("Error while checking updates. Try again?")) return;
            await CheckAppVersion();
        }
    }

    public static void CheckServersFile()
    {
        var path = Path.Combine(App.DataPath, "servers.txt");
        if (!DatabaseHashes.TryGetValue("servers.txt", out var serversHash)) return;
        if (File.Exists(path) && HashUtils.GenerateFileHash(path) == serversHash) return;
        DownloadServersFile();
    }
    public static void CheckDatabaseHash()
    {
        DatabaseHashes.Clear();
        try
        {
            DownloadDatabaseHashes();
        }
        catch (Exception ex)
        {
            Log.F($"Failed to download database hashes. nException: {ex.Message}\n{ex.StackTrace}");
            if (App.SplashScreen.VM.AskUpdate("Failed to download database hashes. Try again?")) CheckDatabaseHash();
        }
    }
    public static bool IsBetaNewer()
    {
        try
        {
            using var c = MiscUtils.GetDefaultHttpClient();

            var req = c.GetStreamAsync(AppVersionBetaUrl);
            req.Wait();
            var st = req.Result;
            var newVersionInfo = new StreamReader(st).ReadLine();
            if (newVersionInfo == null) return false;
            if (Version.Parse(newVersionInfo) > Assembly.GetExecutingAssembly().GetName().Version) return true;
        }
        catch (Exception e)
        {
            Log.F($"[IsBetaNewer] Failed to check beta version {e}");
        }
        return false;
    }
    public static void UpdateDatabase(string relativePath)
    {
        // example https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/acc_benefits/acc_benefits-EU-EN.tsv
        try
        {
            var url = $"https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/menma/{relativePath.Replace("\\", "/")}";
            var destPath = Path.Combine(App.DataPath, relativePath);
            var destDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destDir) && destDir != null) Directory.CreateDirectory(destDir);
            using var c = MiscUtils.GetDefaultHttpClient();
            c.DownloadFileAsync(url, destPath).Wait();
        }
        catch
        {
            var res = TccMessageBox.Show(SR.DbDownloadFailed(Path.GetFileNameWithoutExtension(relativePath)), MessageBoxType.ConfirmationWithYesNo);
            if (res == MessageBoxResult.Yes) UpdateDatabase(relativePath);
        }
    }
    public static void StartPeriodicCheck()
    {
        _checkTimer.Elapsed += CheckTimer_Elapsed;
        _checkTimer.Start();
    }
    public static async void ForceUpdateToBeta()
    {
        using var c = MiscUtils.GetDefaultHttpClient();
        try
        {
            var vp = new VersionParser(forceBeta: true);
            if (!vp.Valid) return;

            Log.N("TCC update manager", "Download started", NotificationType.Info, 3000);
            await c.DownloadFileAsync(vp.NewVersionUrl, "update.zip");

            Log.N("TCC update manager", "Extracting zip", NotificationType.Info, 3000);
            if (Directory.Exists(Path.Combine(App.BasePath, "tmp"))) Directory.Delete(Path.Combine(App.BasePath, "tmp"), true);
            ZipFile.ExtractToDirectory("update.zip", Path.Combine(App.BasePath, "tmp"));

            Log.N("TCC update manager", "Moving files", NotificationType.Info, 2000);
            File.Move(Path.Combine(App.BasePath, "tmp/TCCupdater.exe"), Path.Combine(App.BasePath, "TCCupdater.exe"));

            Log.N("TCC update manager", "Starting updater", NotificationType.Info, 1000);
            await Task.Delay(1000).ContinueWith(_ => Process.Start(Path.GetDirectoryName(typeof(App).Assembly.Location) + "/TCCupdater.exe", "update"));
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Log.F($"Error while checking updates. \nException: {ex.Message}\n{ex.StackTrace}");
            if (TccMessageBox.Show(SR.UpdateCheckFailed, MessageBoxType.ConfirmationWithYesNo) != MessageBoxResult.Yes) return;
            ForceUpdateToBeta();
        }
    }
    public static void StopTimer()
    {
        _checkTimer.Stop();
    }
    public static void TryDeleteUpdater()
    {
        try
        {
            //TODO: update for netcore
            File.Delete(Path.Combine(App.BasePath, "TCCupdater.exe"));
        }
        catch
        {
            /* ignored*/
        }
    }

    //---------------------------------------------------------------------
    static async Task Update(string url)
    {
        using var c = new HttpClientProgress();
        try
        {
            App.SplashScreen.VM.BottomText = "Downloading update...";
            c.DownloadFileCompleted += _ => _waitingDownload = false;
            c.DownloadProgressChanged += (read, total) => App.SplashScreen.VM.Progress = (int)(100 * read / (double)total);

            await App.SplashScreen.Dispatcher.InvokeAsync(() =>
            {
                c.DownloadFileAsync(new Uri(url), "update.zip");
            });

            while (_waitingDownload) Thread.Sleep(1000); //only way to wait for downlaod

            App.SplashScreen.VM.BottomText = "Extracting zip...";
            if (Directory.Exists(Path.Combine(App.BasePath, "tmp"))) Directory.Delete(Path.Combine(App.BasePath, "tmp"), true);
            ZipFile.ExtractToDirectory("update.zip", Path.Combine(App.BasePath, "tmp"));

            App.SplashScreen.VM.BottomText = "Moving files...";
            File.Move(Path.Combine(App.BasePath, "tmp/TCCupdater.exe"), Path.Combine(App.BasePath, "TCCupdater.exe"));

            App.SplashScreen.VM.BottomText = "Starting updater...";
            //TODO: update for netcore
            Process.Start(Path.GetDirectoryName(typeof(App).Assembly.Location) + "/TCCupdater.exe", "update");
            Environment.Exit(0);
        }
        catch (Exception e)
        {
            Log.F($"Error while downloading update. \nException:\n{e.Message}\n{e.StackTrace}");
            var res = TccMessageBox.Show(SR.UpdateDownloadFailed, MessageBoxType.ConfirmationWithYesNo);
            if (res != MessageBoxResult.Yes) return;
            await Update(url);
        }
    }

    static void DownloadServersFile()
    {
        if (!Directory.Exists(App.DataPath)) Directory.CreateDirectory(App.DataPath);
        using var c = MiscUtils.GetDefaultHttpClient();
        try
        {
            c.DownloadFileAsync("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/servers.txt",
                Path.Combine(App.DataPath, "servers.txt")).Wait();
        }
        catch
        {
            var res = TccMessageBox.Show(SR.ServersFileDownloadFailed, MessageBoxType.ConfirmationWithYesNo);
            if (res == MessageBoxResult.Yes) DownloadServersFile();
        }
    }

    static void CheckTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _checkTimer.Stop();
        CheckAppVersionPeriodic();
        if (!UpdateAvailable) _checkTimer.Start();
    }

    static void CheckAppVersionPeriodic()
    {
        try
        {
            var vp = new VersionParser();
            if (!vp.Valid || !vp.IsNewer) return;

            UpdateAvailable = true;

            ChatManager.Instance.AddTccMessage($"TCC v{vp.NewVersionNumber} is now available!");
            Log.N("Update manager", $"TCC v{vp.NewVersionNumber} available!", NotificationType.Info, 10000);
        }
        catch (Exception ex)
        {
            Log.F($"{ex.Message}\n{ex.StackTrace}\n{ex.Source}\n{ex}\n{ex.Data}\n{ex.InnerException}\n{ex.TargetSite}");
        }
    }

    static void DownloadDatabaseHashes()
    {
        DatabaseHashes.Clear();
        using var c = MiscUtils.GetDefaultHttpClient();
        var req = c.GetStreamAsync(DatabaseHashFileUrl);
        req.Wait();
        var f = req.Result;
        using var sr = new StreamReader(f);
        var sHashes = sr.ReadToEnd();
        var jHashes = JObject.Parse(sHashes);
        jHashes.Descendants().ToList().ForEach(jDesc =>
        {
            if (jDesc is not JProperty jProp) return;
            DatabaseHashes[jProp.Name] = jProp.Value.ToString();
        });
    }


    class VersionParser
    {
        Version Version => Version.Parse(NewVersionNumber);

        public string NewVersionNumber { get; }
        public string NewVersionUrl { get; }
        public bool IsNewer => Version > Assembly.GetExecutingAssembly().GetName().Version;
        public bool Valid { get; }

        public VersionParser(bool forceBeta = false)
        {
            NewVersionNumber = "";
            NewVersionUrl = "";

            using var c = MiscUtils.GetDefaultHttpClient();
            var req = c.GetStreamAsync(App.Beta || forceBeta ? AppVersionBetaUrl : AppVersionUrl);
            req.Wait();
            var st = req.Result;

            using var sr = new StreamReader(st);
            var nv = sr.ReadLine();
            var url = sr.ReadLine();

            if (nv == null || url == null)
            {
                return;
            }
            NewVersionNumber = nv;
            NewVersionUrl = url;
            Valid = true;
        }
    }
}