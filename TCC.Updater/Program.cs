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
        private static readonly string SourcePath = Path.GetDirectoryName(typeof(Program).Assembly.Location)+ "/tmp";
        private static readonly string DestinationPath = AppDomain.CurrentDomain.BaseDirectory;

        private static void Main(string[] args)
        {
            if (args.All(x => x != "update"))
            {
                MessageBox.Show("This is not meant to be launched manually!", "TCC Updater", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var tries = 0;
            while (true)
            {
                Thread.Sleep(1000);
                var pl = Process.GetProcesses();
                if (pl.All(x => x.ProcessName != "TCC")) break;
                if (tries > 10)
                {
                    Console.WriteLine("\nForce closing TCC...");
                    var tcc = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == "TCC");
                    tcc?.Kill();
                    break;
                }
                Console.Write($"\rWaiting for TCC to close... {10 - tries} ");
                tries++;
            }
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
                Console.WriteLine($"Copied file {Path.GetFileName(newPath)}");
            }

            Directory.Delete(SourcePath, true);
            Process.Start("explorer.exe", "https://github.com/Foglio1024/Tera-custom-cooldowns/releases");
            Process.Start(Path.GetDirectoryName(typeof(Program).Assembly.Location)+ "/TCC.exe");
            Environment.Exit(0);
        }
    }
}
