using System;
using System.IO;
using TCC.ViewModels;

namespace TCC
{
    public static class Log
    {
        public static void CW(string line)
        {
#if DEBUG
            Console.WriteLine(line);
#endif
        }

        public static void C(string line)
        {
#if DEBUG
            ChatWindowManager.Instance.AddTccMessage(line);
#endif
        }

        public static void F(string line)
        {
            try
            {
                File.AppendAllText(Path.Combine(App.BasePath, "error.log"), $"############### {App.AppVersion} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ###############\n{line}\n\n");
            }
            catch { }
        }
        public static void F(string filename, string line)   //by HQ 20181228
        {
            try
            {
                File.AppendAllText(Path.Combine(App.BasePath, filename), $"############### {App.AppVersion} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ###############\n{line}\n\n");
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
