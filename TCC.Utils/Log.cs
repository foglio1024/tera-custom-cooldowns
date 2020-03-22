using System;
using System.Diagnostics;
using System.IO;
using TCC.Data;

namespace TCC.Utils
{
    public static class Log
    {
        public static event Action<ChatChannel, string, string> NewChatMessage;
        public static event Func<string, string, NotificationType, int, NotificationTemplate, int> NewNotification;

        private static string _logPath = "logs";
        private static string _version = "";

        public static void CW(string line)
        {
            Debug.WriteLine(line);
            //if (Debugger.IsAttached) Debug.WriteLine(line);
            //else
            //{
                //Console.WriteLine(line);
            //}
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

        public static int N(string title, string line, NotificationType type, int duration = -1, NotificationTemplate template = NotificationTemplate.Default)
        {
            return NewNotification?.Invoke(title, line, type, duration, template) ?? -1;
        }

        public static void Config(string path, string version)
        {
            _logPath = path;
            _version = version;
        }

        public static void Chat(ChatChannel channel, string author, string message)
        {
            if (!message.StartsWith("<font")) message = ChatUtils.Font(message);
            NewChatMessage?.Invoke(channel, message, author);
        }
    }
}
