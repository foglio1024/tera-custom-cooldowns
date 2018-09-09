using System;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC
{
    public static class UpdateManager
    {
        private static System.Timers.Timer checkTimer;
        private static string databasePath = "https://github.com/Foglio1024/tera-used-icons/archive/master.zip";
        private static string databaseVersion = "https://raw.githubusercontent.com/Foglio1024/tera-used-icons/master/current_version";
        private static string appVersion = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/master/version";
        private static string baseDatabaseDir = "tera-used-icons-master";
        public static void CheckDatabaseVersion()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                try
                {
                    var st = c.OpenRead(databaseVersion);
                    if (st != null)
                    {
                        var sr = new StreamReader(st);

                        var newVersion = Convert.ToInt32(sr.ReadLine());
                        var currentVersion = 0;
                        if (File.Exists("resources/images/current_version"))
                        {
                            using (var str = File.OpenText("resources/images/current_version"))
                            {
                                currentVersion = Convert.ToInt32(str.ReadLine());
                                str.Close();
                            }

                        }

                        if (newVersion > currentVersion)
                        {
                            if (App.SplashScreen.AskUpdate($"Icons database v{newVersion} available. Download now?"))
                            {
                                DownloadDatabase();
                                ExtractDatabase();
                            }
                            //if (MessageBox.Show($"Icons database v{newVersion} available. Download now?", "TCC", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            //{
                            //    DownloadDatabase();
                            //    ExtractDatabase();
                            //}
                        }
                    }
                }
                catch (Exception)
                {
                    if (App.SplashScreen.AskUpdate("Error while checking database update. Try again?"))
                    {
                        CheckDatabaseVersion();
                    }
                    //MessageBox.Show("Error while checking database updates.", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private static void DownloadDatabase()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var c = new WebClient())
            {
                var ready = false;
                c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                c.DownloadProgressChanged += App.SplashScreen.UpdateProgress;
                c.DownloadFileCompleted += (s, ev) => ready = true;
                try
                {
                    App.SplashScreen.SetText("Downloading database...");
                    App.SplashScreen.Dispatcher.Invoke(() => c.DownloadFileAsync(new Uri(databasePath), "icons.zip"));
                    while (!ready) Thread.Sleep(1);
                    App.SplashScreen.SetText("Downloading database... Done.");

                }
                catch (Exception)
                {
                    if (App.SplashScreen.AskUpdate("Error while downloading database. Try again?"))
                    {
                        DownloadDatabase();
                    }

                    //MessageBox.Show("Couldn't download database.", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);                    
                }
            }
        }

        private static void ExtractDatabase()
        {
            if (Directory.Exists(baseDatabaseDir))
            {
                Directory.Delete(baseDatabaseDir, true);
            }
            App.SplashScreen.SetText("Extracting database...");

            ZipFile.ExtractToDirectory("icons.zip", AppDomain.CurrentDomain.BaseDirectory);
            App.SplashScreen.SetText("Extracting database... Done.");

            foreach (var dirPath in Directory.GetDirectories(baseDatabaseDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(baseDatabaseDir, "resources/images"));
            }
            App.SplashScreen.SetText("Copying files...");

            foreach (var newPath in Directory.GetFiles(baseDatabaseDir, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(baseDatabaseDir, "resources/images"), true);
            }
            App.SplashScreen.SetText("Copying files... Done.");

            CleanTempDatabase();

            App.SplashScreen.SetText("Database updated successfully.");

            //MessageBox.Show("Database updated.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void CleanTempDatabase()
        {
            try
            {
                Directory.Delete(baseDatabaseDir, true);
                File.Delete("icons.zip");
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public static void StartCheck()
        {
            checkTimer = new System.Timers.Timer(60 * 10 * 1000);
            checkTimer.Elapsed += CheckTimer_Elapsed;
            checkTimer.Start();
        }

        private static void CheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkTimer.Stop();
            CheckAppVersionPeriodic();
            checkTimer.Start();
        }

        private static void CheckAppVersionPeriodic()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                try
                {
                    var st = c.OpenRead(appVersion);
                    if (st != null)
                    {
                        var sr = new StreamReader(st);
                        var newVersionInfo = sr.ReadLine();
                        var newVersionUrl = sr.ReadLine();

                        if (newVersionInfo != null)
                        {
                            var v = Version.Parse(newVersionInfo);
                            if (v > Assembly.GetExecutingAssembly().GetName().Version)
                            {
                                ChatWindowManager.Instance.AddTccMessage($"TCC v{newVersionInfo} available!");
                                WindowManager.FloatingButton.NotifyExtended("Update manager", $"TCC v{newVersionInfo} available!", NotificationType.Success);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/update-check-error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
                             ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                             "\r\n" + ex.TargetSite);
                }
            }
        }
        public static void CheckAppVersion()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                try
                {
                    var st = c.OpenRead(appVersion);
                    if (st != null)
                    {
                        var sr = new StreamReader(st);
                        var newVersionInfo = sr.ReadLine();
                        var newVersionUrl = sr.ReadLine();

                        if (newVersionInfo != null)
                        {
                            var v = Version.Parse(newVersionInfo);
                            if (v > Assembly.GetExecutingAssembly().GetName().Version)
                            {
                                if (App.SplashScreen.AskUpdate($"TCC v{newVersionInfo} available. Download now?"))
                                {
                                    Update(newVersionUrl);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/update-check-error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
                             ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                             "\r\n" + ex.TargetSite);
                    //MessageBox.Show("Error while checking updates. More info in update-check-error.txt", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (App.SplashScreen.AskUpdate("Error while checking updates. Try again?"))
                    {
                        CheckAppVersion();
                    }
                }
            }
        }


        private static void Update(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var c = new WebClient())
            {
                try
                {
                    c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                    App.SplashScreen.SetText("Downloading update...");
                    var ready = false;
                    c.DownloadProgressChanged += App.SplashScreen.UpdateProgress;
                    c.DownloadFileCompleted += (s, ev) => ready = true;
                    App.SplashScreen.Dispatcher.Invoke(() => c.DownloadFileAsync(new Uri(url), "update.zip"));

                    while (!ready) Thread.Sleep(1);

                    App.SplashScreen.SetText("Extracting zip...");
                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/tmp")) Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "/tmp", true);
                    ZipFile.ExtractToDirectory("update.zip", AppDomain.CurrentDomain.BaseDirectory + "/tmp");
                    App.SplashScreen.SetText("Moving files...");

                    File.Move(AppDomain.CurrentDomain.BaseDirectory + "/tmp/TCCupdater.exe", AppDomain.CurrentDomain.BaseDirectory + "/TCCupdater.exe");
                    App.SplashScreen.SetText("Starting updater...");

                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/TCCupdater.exe", "update");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/update-error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
         ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
         "\r\n" + ex.TargetSite);
                    //           MessageBox.Show("Error while checking updates. More info in update-error.txt", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);

                    //           MessageBox.Show("Couldn't download update.", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);

                    if (App.SplashScreen.AskUpdate("Error while downloading update. Try again? If the error perists download TCC manually."))
                    {
                        Update(url);
                    }
                }
            }

        }

        internal static void StopTimer()
        {
            checkTimer?.Stop();
        }
    }
}
