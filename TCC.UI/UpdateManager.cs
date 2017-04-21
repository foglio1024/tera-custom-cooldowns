using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Compression;
using System.IO;

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
                c.DownloadFile(databaseVersion, "newDbVer");
            }
            int v = Convert.ToInt32(File.ReadAllText("resources/images/current_version"));
            int newVer = Convert.ToInt32(File.ReadAllText("newDbVer"));

            if(v < newVer)
            {
                //update
                if(MessageBox.Show(String.Format("Updated icons database available (v{0}). Download now?", newVer), "TCC", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DownloadDatabase();
                    ExtractDatabase();
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

            }
        }
    }
}
