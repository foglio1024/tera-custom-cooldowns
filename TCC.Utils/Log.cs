using System;
using System.Diagnostics;
using System.IO;

namespace TCC.Utils
{
    public static class Log
    {
        public static event Action<string> NewChatLogMessage;
        public static event Func<string, string, NotificationType, uint, NotificationTemplate, int > NewNotification;

        private static string _logPath = "logs";
        private static string _version = "";

        [Conditional("DEBUG")]
        public static void CW(string line)
        {
            Console.WriteLine(line);
        }

        [Conditional("DEBUG")]
        public static void C(string line)
        {
            NewChatLogMessage?.Invoke(line);
            //App.BaseDispatcher.InvokeAsync(() => ChatWindowManager.Instance.AddTccMessage(line));
        }

        public static void F(string line, string fileName = "error.log")
        {
            try
            {
                if (!Directory.Exists(_logPath))
                    Directory.CreateDirectory(_logPath);
                File.AppendAllText(Path.Combine(_logPath, fileName), $"############### {_version} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ###############\n{line}\n\n");
            }
            catch { }
        }

        public static int N(string title, string line, NotificationType type, uint duration = 4000U, NotificationTemplate template = NotificationTemplate.Default)
        {
            return NewNotification?.Invoke(title, line, type, duration, template) ?? -1;
        }
        public static void All(string s)
        {
            CW(s);
            C(s);
        }

        public static void Config(string path, string version)
        {
            _logPath = path;
            _version = version;
        }
    }
}
