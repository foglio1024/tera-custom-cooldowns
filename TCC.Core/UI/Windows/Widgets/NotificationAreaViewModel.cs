using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Nostrum;
using TCC.Settings.WindowSettings;
using TCC.Utils;
using TCC.ViewModels;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;
using TeraPacketParser.TeraCommon.Game;

namespace TCC.UI.Windows.Widgets
{
    public class NotificationAreaViewModel : TccWindowViewModel
    {
        private static int _id;
        private readonly ConcurrentQueue<NotificationInfoBase> _queue;
        public TSObservableCollection<NotificationInfoBase> Notifications { get; }

        public NotificationAreaViewModel(WindowSettingsBase settings) : base(settings)
        {
            _queue = new ConcurrentQueue<NotificationInfoBase>();
            Notifications = new TSObservableCollection<NotificationInfoBase>(Dispatcher);
            Log.NewNotification += Enqueue;
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.Processor.Hook<C_LOGIN_ARBITER>(OnLoginArbiter);
            PacketAnalyzer.Sniffer.NewConnection += OnNewConnection;
        }

        private void OnNewConnection(Server srv)
        {
            Enqueue("TCC", SR.ConnectedToServer(srv.Name), NotificationType.Success,
                template: NotificationTemplate.Progress,
                forcedId: 10241024);

            Task.Delay(10000).ContinueWith(_ =>
            {
                var notif = GetNotification<ProgressNotificationInfo>(10241024);
                if (notif == null) return;
                notif.Message = $"Connection timed out. Reconnect or restart TCC if the issue persists.";
                notif.NotificationType = NotificationType.Warning;
                notif.Dispose(20000);

            });
            //Log.N("TCC", SR.ConnectedToServer(srv.Name), NotificationType.Success, forcedId: 10241024);
        }
        private void OnLoginArbiter(C_LOGIN_ARBITER obj)
        {
            var notif = GetNotification<ProgressNotificationInfo>(10241024);
            if (notif == null) return;
            notif.Message += $"\nRelease Version: {PacketAnalyzer.Factory!.ReleaseVersion / 100}.{PacketAnalyzer.Factory!.ReleaseVersion % 100}"; //by HQ 20190209
            notif.Dispose(3000);
        }

        private void CheckShow()
        {
            Dispatcher.Invoke(() =>
            {
                while (Notifications.Count < ((NotificationAreaSettings)Settings!).MaxNotifications)
                {
                    if (_queue.IsEmpty) break;
                    if (!_queue.TryDequeue(out var next)) continue;
                    if (!Pass(next)) continue;
                    Notifications.Add(next);
                }
            });
        }

        private bool Pass(NotificationInfoBase infoBase)
        {
            return Notifications.ToSyncList().All(n => n.Message != infoBase.Message);
        }

        private int Enqueue(string title, string message, NotificationType type, int secDuration = -1, NotificationTemplate template = NotificationTemplate.Default, int forcedId = -1)
        {
            Dispatcher.Invoke(() =>
            {
                var id = forcedId != -1 ? forcedId : _id;
                if (secDuration == -1) secDuration = ((NotificationAreaSettings)Settings!).DefaultNotificationDuration * 1000;
                switch (template)
                {
                    case NotificationTemplate.Progress:
                        _queue.Enqueue(new ProgressNotificationInfo(id, title, message, type, secDuration, template));
                        break;
                    default:
                        _queue.Enqueue(new NotificationInfoBase(id, title, message, type, secDuration, template));
                        break;
                }
                CheckShow();
            });
            return forcedId != -1 ? forcedId : _id++;
        }

        public void DeleteNotification(NotificationInfoBase dc)
        {
            Dispatcher.InvokeAsync(() =>
            {
                Notifications.Remove(dc);
                CheckShow();
            }, DispatcherPriority.Background);
        }

        public T? GetNotification<T>(int notifId) where T : NotificationInfoBase
        {
            var ret = Notifications.ToSyncList().FirstOrDefault(x => x.Id == notifId);
            if (ret != null) return (T)ret;
            ret = _queue.ToArray().FirstOrDefault(x => x.Id == notifId);
            return (T?)ret;
        }
    }
}