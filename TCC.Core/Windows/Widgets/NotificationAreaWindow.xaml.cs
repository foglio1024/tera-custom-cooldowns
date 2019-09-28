using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.Data;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public class NotificationInfo
    {
        public string Title { get; }
        public string Message { get; }
        public NotificationType NotificationType { get; }
        public uint Duration { get; }
        public string TimeStamp { get; }
        public NotificationInfo(string title, string message, NotificationType type, uint duration)
        {
            Title = title;
            Message = message;
            NotificationType = type;
            Duration = duration;
            TimeStamp = DateTime.Now.ToString("HH:mm:ss");
        }
    }
    public class NotificationAreaViewModel : TccWindowViewModel
    {
        private readonly ConcurrentQueue<NotificationInfo> _queue;
        public SynchronizedObservableCollection<NotificationInfo /*TODO: use VM if needed*/> Notifications { get; }
        public NotificationAreaViewModel(WindowSettings settings) : base(settings)
        {
            _queue = new ConcurrentQueue<NotificationInfo>();
            Notifications = new SynchronizedObservableCollection<NotificationInfo>(Dispatcher);
        }

        private void CheckShow()
        {
            while (Notifications.Count < ((NotificationAreaSettings)Settings).MaxNotifications)
            {
                if (_queue.IsEmpty) break;
                if (!_queue.TryDequeue(out var next)) continue;
                if (!Pass(next)) continue;
                Console.WriteLine($"Adding {next.Message}");
                Notifications.Add(next);
            }
        }
        private bool Pass(NotificationInfo info)
        {
            return Notifications.ToSyncList().All(n => n.Message != info.Message);
        }
        public void Enqueue(string title, string message, NotificationType type, uint duration = 4000U)
        {
            _queue.Enqueue(new NotificationInfo(title, message, type, duration));
            CheckShow();
        }

        public void DeleteNotification(NotificationInfo dc)
        {
            Notifications.Remove(dc);
            CheckShow();
        }
    }
    public partial class NotificationAreaWindow
    {
        public NotificationAreaWindow(NotificationAreaViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(vm.Settings);
            MainContent.Opacity = 1;
        }
    }
}
