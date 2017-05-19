using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Updater
{
    class Program
    {
        static string SourcePath = Environment.CurrentDirectory + "/tmp";
        static string DestinationPath = Environment.CurrentDirectory;

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
            Process.Start(Environment.CurrentDirectory + "/TCC.exe");
            Environment.Exit(0);
        }
    }
}
