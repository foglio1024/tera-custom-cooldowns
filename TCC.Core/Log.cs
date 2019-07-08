using System;
using System.Diagnostics;
using System.IO;
using TCC.ViewModels;

namespace TCC
{
    public static class Log
    {
        [Conditional("DEBUG")]
        public static void CW(string line)
        {
            Console.WriteLine(line);
        }

        [Conditional("DEBUG")]
        public static void C(string line)
        {
            App.BaseDispatcher.BeginInvoke(new Action(() => { ChatWindowManager.Instance.AddTccMessage(line); }));
        }

        public static void F(string line, string fileName = "error.log")
        {
            try
            {
                if (!Directory.Exists(Path.Combine(App.BasePath, "logs")))
                    Directory.CreateDirectory(Path.Combine(App.BasePath, "logs"));
                File.AppendAllText(Path.Combine(App.BasePath, "logs", fileName), $"############### {App.AppVersion} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ###############\n{line}\n\n");
            }
            catch { }
        }

        public static void All(string s)
        {
            CW(s);
            C(s);
        }
    }
}
