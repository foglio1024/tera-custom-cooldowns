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
            App.BaseDispatcher.BeginInvoke(new Action(() => { ChatWindowManager.Instance.AddTccMessage(line); }));
#endif
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
        //public static void F(string filename, string line)   //by HQ 20181228
        //{
        //    try
        //    {
        //        File.AppendAllText(Path.Combine(App.BasePath, "logs", filename), $"############### {App.AppVersion} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ###############\n{line}\n\n");
        //    }
        //    catch { }
        //}

        public static void All(string s)
        {
            CW(s);
            C(s);
        }
    }
}
