using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Settings;
using TCC.Utils;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public class NotificationAreaViewModel : TccWindowViewModel
    {
        private readonly ConcurrentQueue<NotificationInfo> _queue;
        public TSObservableCollection<NotificationInfo> Notifications { get; }

        public NotificationAreaViewModel(WindowSettings settings) : base(settings)
        {
            _queue = new ConcurrentQueue<NotificationInfo>();
            Notifications = new TSObservableCollection<NotificationInfo>(Dispatcher);
            Log.NewNotification += Enqueue;
        }

        private void CheckShow()
        {
            Dispatcher.InvokeAsync(() =>
            {
                while (Notifications.Count < ((NotificationAreaSettings) Settings).MaxNotifications)
                {
                    if (_queue.IsEmpty) break;
                    if (!_queue.TryDequeue(out var next)) continue;
                    if (!Pass(next)) continue;
                    Notifications.Add(next);
                }
            });
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
            Dispatcher.InvokeAsync(() =>
            {
                Notifications.Remove(dc);
                CheckShow();
            }, DispatcherPriority.Background);
        }
    }
}