using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace TCC.Updater
{
    internal class Program
    {
        private static string SourcePath = AppDomain.CurrentDomain.BaseDirectory + "/tmp";
        private static string DestinationPath = AppDomain.CurrentDomain.BaseDirectory;

        private static void Main(string[] args)
        {
            if (!args.Any(x => x == "update"))
            {
                MessageBox.Show("This is not meant to be launched manually!", "TCC Updater", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Thread.Sleep(2000);
            //Create all of the directories
            foreach (var dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (newPath.Contains(@"\config\")) continue;
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
                Console.WriteLine("Copied file {0}", Path.GetFileName(newPath));
            }

            Directory.Delete(SourcePath, true);
            Process.Start("explorer.exe", "https://github.com/Foglio1024/Tera-custom-cooldowns/releases");
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/TCC.exe");
            Environment.Exit(0);
        }
    }
}
