using System;
using System.Net;
using System.Windows;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace TCC
{
    public static class UpdateManager
    {
        static string databasePath = "https://github.com/Foglio1024/tera-used-icons/archive/master.zip";
        static string databaseVersion = "https://raw.githubusercontent.com/Foglio1024/tera-used-icons/master/current_version";
        static string baseDatabaseDir = "tera-used-icons-master";
        public static void CheckDatabaseVersion()
        {
            using (WebClient c = new WebClient())
            {
                try
                {
                    var st = c.OpenRead(databaseVersion);
                    if (st != null)
                    {
                        StreamReader sr = new StreamReader(st);

                        int newVersion = Convert.ToInt32(sr.ReadLine());
                        int currentVersion = 0;
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
        static void DownloadDatabase()
        {
            using (WebClient c = new WebClient())
            {
                bool ready = false;
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
        static void ExtractDatabase()
        {
            if (Directory.Exists(baseDatabaseDir))
            {
                Directory.Delete(baseDatabaseDir, true);
            }
            App.SplashScreen.SetText("Extracting database...");

            ZipFile.ExtractToDirectory("icons.zip", Environment.CurrentDirectory);
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
        static void CleanTempDatabase()
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

        static string appVersion = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/master/version";

        public static void CheckAppVersion()
        {
            using (WebClient c = new WebClient())
            {

                try
                {
                    var st = c.OpenRead(appVersion);
                    if (st != null)
                    {
                        StreamReader sr = new StreamReader(st);
                        string newVersionInfo = sr.ReadLine();
                        string newVersionUrl = sr.ReadLine();

                        if (newVersionInfo != null)
                        {
                            var v = Version.Parse(newVersionInfo);
                            if (v > Assembly.GetExecutingAssembly().GetName().Version)
                            {
                                if (App.SplashScreen.AskUpdate($"TCC v{newVersionInfo} available. Download now?"))
                                {
                                    Update(newVersionUrl);
                                }
                                //if (MessageBox.Show($"TCC v{newVersionInfo} available. Download now?", "TCC", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                //{
                                //    Update(newVersionUrl);
                                //}
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Environment.CurrentDirectory + "/update-check-error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
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

            using (var c = new WebClient())
            {
                try
                {
                    App.SplashScreen.SetText("Downloading update...");
                    bool ready = false;
                    c.DownloadProgressChanged += App.SplashScreen.UpdateProgress;
                    c.DownloadFileCompleted += (s, ev) => ready = true;
                    App.SplashScreen.Dispatcher.Invoke(() => c.DownloadFileAsync(new Uri(url), "update.zip"));

                    while (!ready) Thread.Sleep(1);

                    App.SplashScreen.SetText("Extracting zip...");

                    ZipFile.ExtractToDirectory("update.zip", Environment.CurrentDirectory + "/tmp");
                    App.SplashScreen.SetText("Moving files...");

                    File.Move(Environment.CurrentDirectory + "/tmp/TCCupdater.exe", Environment.CurrentDirectory + "/TCCupdater.exe");
                    App.SplashScreen.SetText("Starting updater...");

                    Process.Start(Environment.CurrentDirectory + "/TCCupdater.exe");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Environment.CurrentDirectory + "/update-error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
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

    }
}
