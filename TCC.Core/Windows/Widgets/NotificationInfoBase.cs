using System;
using System.Threading.Tasks;
using FoglioUtils;
using TCC.Utils;

namespace TCC.Windows.Widgets
{
    public class ProgressNotificationInfo : NotificationInfoBase
    {
        public event Action Disposed;
        public event Action<int> Disposing;
        private double _progress;
        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress == value) return;
                _progress = value;
                N();
            }
        }

        public ProgressNotificationInfo(int id, string title, string message, NotificationType type, uint duration, NotificationTemplate template) : base(id, title, message, type, duration, template)
        {

        }

        public void Dispose(int msDelay)
        {
            Disposing?.Invoke(msDelay);
            if (msDelay == 0)
            {
                Disposed?.Invoke();
            }
            else
            {
                Task.Delay(msDelay).ContinueWith(t => Disposed?.Invoke());
            }
        }
    }
    public class NotificationInfoBase : TSPropertyChanged
    {
        private string _message;
        private NotificationType _notificationType;

        public int Id { get; }
        public string Title { get; }
        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) return;
                _message = value;
                N();
            }
        }

        public NotificationType NotificationType
        {
            get => _notificationType;
            set
            {
                if (_notificationType == value) return;
                _notificationType = value;
                N();
            }
        }

        public NotificationTemplate NotificationTemplate { get; }
        public uint Duration { get; }
        public string TimeStamp { get; }
        public string Version => App.AppVersion;

        public NotificationInfoBase(int id, string title, string message, NotificationType type, uint duration, NotificationTemplate template)
        {
            Id = id;
            Title = title;
            Message = message;
            _notificationType = type;
            Duration = duration;
            TimeStamp = DateTime.Now.ToString("HH:mm:ss");
            NotificationTemplate = template;
        }
    }
}