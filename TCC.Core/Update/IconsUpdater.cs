using Newtonsoft.Json.Linq;
using Nostrum;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TCC.Data;
using TCC.UI;
using TCC.UI.Windows;
using TCC.UI.Windows.Widgets;
using TCC.Utilities;
using TCC.Utils;

namespace TCC.Update;

public class IconsUpdater
{
    static string DownloadedIconsDir => Path.Combine(App.BasePath, "tera-used-icons-master");
    const string IconsUrl = "https://github.com/Foglio1024/tera-used-icons/archive/master.zip";

    ProgressNotificationInfo? _n;

    public async Task CheckForUpdates()
    {
        var path = Path.Combine(App.ResourcesPath, "images");
        if (!Directory.Exists(path) || !Directory.EnumerateDirectories(path).Any())
        {
            await DownloadArchive();
        }
        else
        {
            await CheckHashes();
        }
    }

    async Task CheckHashes()
    {
        string hashFile;
        try
        {
            using var c = MiscUtils.GetDefaultHttpClient();
            hashFile = await c.GetStringAsync(new Uri("https://github.com/Foglio1024/tera-used-icons/raw/master/hashes.json"));
        }
        catch (Exception e)
        {
            Log.N("TCC icon updater", "Failed to retrieve icons hash file.\nIcons won't be updated.'", NotificationType.Error);
            Log.F($"Failed to retrieve hashes.json: {e}");
            return;
        }
        var jHash = JObject.Parse(hashFile);
        var mismatched = new List<string>();
        var dirs = jHash.Children().Select(j => (JProperty)j).ToList();
        dirs.ForEach(jDir =>
        {
            var dirPath = Path.Combine(App.ResourcesPath, "images", jDir.Name);
            ((JObject)jDir.Value).Children().Select(j => (JProperty)j).ToList().ForEach(jFile =>
            {
                var filePath = Path.Combine(dirPath, jFile.Name);
                var remoteHash = jFile.Value.ToString();
                var localHash = HashUtils.GenerateFileHash(filePath);

                if (File.Exists(filePath) && localHash == remoteHash) return;
                mismatched.Add($"{dirPath}/{jFile.Name}");
            });
        });

        if (mismatched.Count > 100)
        {
            var ni = Log.N("TCC icon updater", "Many icons are missing, downloading the whole archive...", NotificationType.Info, template: NotificationTemplate.Progress);
            _n = WindowManager.ViewModels.NotificationAreaVM.GetNotification<ProgressNotificationInfo>(ni);
            await DownloadArchive();
        }
        else if (mismatched.Count > 0)
        {
            var ni = Log.N("TCC icon updater", $"Updating {mismatched.Count} icons...", NotificationType.Info, template: NotificationTemplate.Progress);
            _n = WindowManager.ViewModels.NotificationAreaVM.GetNotification<ProgressNotificationInfo>(ni);
            await Task.Run(() => DownloadMissingIcons(mismatched));
        }
        //Log.N("TCC icon updater", "All icons are up to date.", NotificationType.Success);
    }

    async Task DownloadArchive()
    {
        using var c = new HttpClientProgress();

        c.DownloadProgressChanged += (read, total) =>
        {
            if (total == -1) total = 71000000;
            var perc = read * 100 / (double)total;
            if (_n == null) return;
            _n.Progress = perc;
            _n.Message =
                $"Downloading icons...\n({read / (1024 * 1024D):N1}/{total / (1024 * 1024D):N1}MB)";
        };
        c.DownloadFileCompleted += async (success) =>
        {
            if (!success)
            {
                var res = TccMessageBox.Show(SR.IconDownloadFailed, MessageBoxType.ConfirmationWithYesNo);
                if (res == MessageBoxResult.Yes) await DownloadArchive();
            }
            else
            {
                if (_n != null)
                {
                    _n.Message = "Download completed.";
                    _n.Progress = 0;
                }
                Extract();
            }
        };
        try
        {
            if (_n == null)
            {
                var notifId = Log.N("TCC update manager", "Downloading icons...", NotificationType.Info, template: NotificationTemplate.Progress);
                _n = WindowManager.ViewModels.NotificationAreaVM.GetNotification<ProgressNotificationInfo>(notifId);
            }

            await c.DownloadFileAsync(new Uri(IconsUrl), Path.Combine(App.BasePath, "icons.zip"));
        }
        catch (Exception)
        {
            var res = TccMessageBox.Show(SR.IconDownloadFailed, MessageBoxType.ConfirmationWithYesNo);
            if (res == MessageBoxResult.Yes) await DownloadArchive();
        }
    }

    void DownloadMissingIcons(List<string> missing)
    {
        var idx = 1;
        var fails = 0;
        foreach (var icon in missing)
        {
            try
            {
                var splitPath = icon.Split('\\');
                var dir = splitPath.Last().Split('/')[0];
                var iconName = splitPath.Last().Split('/')[1];
                var url = $"https://github.com/Foglio1024/tera-used-icons/raw/master/{dir}/{iconName}";
                using var c = MiscUtils.GetDefaultHttpClient();
                c.DownloadFileAsync(url, Path.Combine(App.ResourcesPath, "images", dir, iconName)).Wait();
                if (_n == null) continue;
                _n.Progress = idx * 100 / (double)missing.Count;
                _n.Message = $"Updating icons... ({idx}/{missing.Count})";
            }
            catch (Exception e)
            {
                Log.F($"Failed to update {missing}: {e}");
                fails++;
            }
            finally
            {
                idx++;
            }
        }

        if (_n == null) return;
        _n.NotificationType = fails > 0 ? NotificationType.Warning : NotificationType.Success;
        _n.Message = fails > 0
            ? $"{missing.Count} icons successfully updated ({fails} failed)"
            : $"{missing.Count} icons successfully updated.";
        _n.Dispose(2000);
    }

    void Extract()
    {
        try
        {
            if (_n != null) _n.Message = "Extracting icons...";
            if (Directory.Exists(DownloadedIconsDir)) Directory.Delete(DownloadedIconsDir, true);
            var imagesPath = Path.Combine(App.ResourcesPath, "images");
            if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
            ZipFile.ExtractToDirectory(Path.Combine(App.BasePath, "icons.zip"), App.BasePath);
            if (_n != null) _n.Message = "Creating directories...";
            Directory.GetDirectories(DownloadedIconsDir, "*", SearchOption.AllDirectories).ToList().ForEach(
                dirPath =>
                {
                    var dir = Path.GetFileName(dirPath);
                    Directory.CreateDirectory(Path.Combine(imagesPath,
                        dir ?? throw new InvalidOperationException()));
                });
            if (_n != null) _n.Message = "Copying icons...";
            var paths = Directory.GetFiles(DownloadedIconsDir, "*.*", SearchOption.AllDirectories).ToList();
            var count = 0;
            var total = paths.Count;
            paths.ForEach(newPath =>
            {
                try
                {
                    File.Copy(newPath.Replace("\\", "/"),
                        Path.Combine(App.ResourcesPath, "images",
                            newPath.Replace(DownloadedIconsDir + "\\", "").Replace("\\", "/")), true);
                }
                catch (Exception e)
                {
                    Log.F("Failed to copy icon " + newPath + "\n" + e);
                }

                if (_n == null) return;
                _n.Message = $"Copying icons...\n({++count}/{total})";
                _n.Progress = count * 100 / (double)total;
            });
            if (_n != null)
            {
                _n.Progress = 0;
                _n.NotificationType = NotificationType.Success;
                _n.Message = "Icons update completed successfully.";
            }
            CleanTempIcons();
            _n?.Dispose(4000);
        }
        catch
        {
            var res = TccMessageBox.Show(SR.IconExtractFailed, MessageBoxType.ConfirmationWithYesNo);
            if (res == MessageBoxResult.Yes)
                Extract();
            else
            {
                if (_n != null)
                {
                    _n.Message = "Icons update aborted.";
                    _n.Dispose(4000);
                }
            }
        }
    }

    static void CleanTempIcons()
    {
        try
        {
            Directory.Delete(DownloadedIconsDir, true);
            File.Delete("icons.zip");
        }
        catch
        {
            // ignored
        }
    }
}