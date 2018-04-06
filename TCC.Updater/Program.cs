using System;
using System.Diagnostics;
using System.IO;

namespace TCC.Updater
{
    class Program
    {
        static string SourcePath = AppDomain.CurrentDomain.BaseDirectory + "/tmp";
        static string DestinationPath = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            //Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (newPath.Contains(@"\config\")) continue;
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
                Console.WriteLine("Copied file {0}", Path.GetFileName(newPath));
            }

            Directory.Delete(SourcePath, true);
            Process.Start("explorer.exe","https://github.com/Foglio1024/Tera-custom-cooldowns/releases");
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/TCC.exe");
            Environment.Exit(0);
        }
    }
}
