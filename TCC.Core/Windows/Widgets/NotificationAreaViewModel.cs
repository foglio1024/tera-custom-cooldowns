﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Threading;
using TCC.Data;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public class NotificationAreaViewModel : TccWindowViewModel
    {
        private readonly ConcurrentQueue<NotificationInfo> _queue;
        public SynchronizedObservableCollection<NotificationInfo> Notifications { get; }

        public NotificationAreaViewModel(WindowSettings settings) : base(settings)
        {
            _queue = new ConcurrentQueue<NotificationInfo>();
            Notifications = new SynchronizedObservableCollection<NotificationInfo>(Dispatcher);
        }

        private void CheckShow()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                while (Notifications.Count < ((NotificationAreaSettings) Settings).MaxNotifications)
                {
                    if (_queue.IsEmpty) break;
                    if (!_queue.TryDequeue(out var next)) continue;
                    if (!Pass(next)) continue;
                    Notifications.Add(next);
                }
            }));
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
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Notifications.Remove(dc);
                CheckShow();
            }), DispatcherPriority.Background);
        }
    }
}