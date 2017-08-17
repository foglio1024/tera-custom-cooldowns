using System;
using System.Net;
using System.Windows;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Diagnostics;

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
                            if (MessageBox.Show($"Icons database v{newVersion} available. Download now?", "TCC", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                DownloadDatabase();
                                ExtractDatabase();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error while checking database updates.", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        static void DownloadDatabase()
        {
            using (WebClient c = new WebClient())
            {
                try
                {
                    c.DownloadFile(databasePath, "icons.zip");
                }
                catch (Exception)
                {
                    MessageBox.Show("Couldn't download database.", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);                    
                }
            }
        }
        static void ExtractDatabase()
        {
            if (Directory.Exists(baseDatabaseDir))
            {
                Directory.Delete(baseDatabaseDir, true);
            }

            ZipFile.ExtractToDirectory("icons.zip", Environment.CurrentDirectory);

            foreach (var dirPath in Directory.GetDirectories(baseDatabaseDir,"*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(baseDatabaseDir, "resources/images"));
            }

            foreach (var newPath in Directory.GetFiles(baseDatabaseDir,"*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(baseDatabaseDir, "resources/images"),true);
            }

            CleanTempDatabase();
            MessageBox.Show("Database updated.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                if (MessageBox.Show($"TCC v{newVersionInfo} available. Download now?", "TCC", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    Update(newVersionUrl);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Environment.CurrentDirectory + "/update-check-error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
                             ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                             "\r\n" + ex.TargetSite);
                    MessageBox.Show("Error while checking updates. More info in update-check-error.txt", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private static void Update(string url)
        {
            using (WebClient c = new WebClient())
            {
                try
                {
                    c.DownloadFile(url, "update.zip");
                    ZipFile.ExtractToDirectory("update.zip", Environment.CurrentDirectory + "/tmp");
                    File.Move(Environment.CurrentDirectory + "/tmp/TCCupdater.exe", Environment.CurrentDirectory + "/TCCupdater.exe");
                    Process.Start(Environment.CurrentDirectory + "/TCCupdater.exe");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Environment.CurrentDirectory + "/update-error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
         ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
         "\r\n" + ex.TargetSite);
                    MessageBox.Show("Error while checking updates. More info in update-error.txt", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);

                    MessageBox.Show("Couldn't download update.", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }
    }
}
