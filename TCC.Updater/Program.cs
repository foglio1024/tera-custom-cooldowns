using System;
using System.Collections.Generic;
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
            if (!HasUpdateArg(args)) return;

            WaitForTccExit();

            CreateDirectories();

            ReplaceFiles();

            // delete temp folder
            Directory.Delete(SourcePath, true);
            // open release notes
            Process.Start("explorer.exe", "https://github.com/Foglio1024/Tera-custom-cooldowns/releases");
            // launch TCC
            Process.Start(Path.GetDirectoryName(typeof(Program).Assembly.Location) + "/TCC.exe");
            // exit
            Environment.Exit(0);
        }

        private static void ReplaceFiles()
        {
            foreach (var newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories).Where(p => !p.Contains(@"\config\")))
            {
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
                Console.WriteLine($"Copied file {Path.GetFileName(newPath)}");
            }
        }
        private static void CreateDirectories()
        {
            foreach (var dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }
        }
        private static void WaitForTccExit()
        {
            var pl = Process.GetProcesses();
            var tries = 10;
            while (pl.Any(x => x.ProcessName == "TCC"))
            {
                if (tries >= 0)
                {
                    Console.Write($"\rWaiting for TCC to close... {tries} ");
                    Thread.Sleep(1000);
                    tries--;
                }
                else
                {
                    Console.WriteLine("\nForce closing TCC...");
                    var tcc = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == "TCC");
                    tcc?.Kill();
                    break;
                }
            }
        }
        private static bool HasUpdateArg(IEnumerable<string> args)
        {
            if (args.Any(x => x == "update")) return true;
            MessageBox.Show("This is not meant to be launched manually!", "TCC Updater", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }
}
