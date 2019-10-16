using System;
using TCC.Data;

namespace TCC.Windows.Widgets
{
    public class NotificationInfo
    {
        public string Title { get; }
        public string Message { get; }
        public NotificationType NotificationType { get; }
        public uint Duration { get; }
        public string TimeStamp { get; }
        public string Version => App.AppVersion;

        public NotificationInfo(string title, string message, NotificationType type, uint duration)
        {
            Title = title;
            Message = message;
            NotificationType = type;
            Duration = duration;
            TimeStamp = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}