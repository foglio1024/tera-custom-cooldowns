using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Messages;

namespace TCC.ViewModels
{
    public static class DispatcherExtensions
    {
        public static void InvokeIfRequired(this Dispatcher disp, Action dotIt, DispatcherPriority priority)
        {
            if (disp.Thread != Thread.CurrentThread)
                disp.Invoke(priority, (Delegate)dotIt);
            else
                dotIt();
        }
    }
    public class TSPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string v)
        {
            Application.Current.Dispatcher.InvokeIfRequired(() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v)), DispatcherPriority.DataBind);
        }
    }
    public class SynchronizedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher;
        private readonly ReaderWriterLockSlim _lock;

        public SynchronizedObservableCollection()
        {
            this._dispatcher = Application.Current.Dispatcher;
            this._lock = new ReaderWriterLockSlim();
        }

        protected override void ClearItems()
        {
            this._dispatcher.InvokeIfRequired((Action)(() =>
            {
                this._lock.EnterWriteLock();
                try
                {
                    base.ClearItems();
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }), DispatcherPriority.DataBind);
        }

        protected override void InsertItem(int index, T item)
        {
            this._dispatcher.InvokeIfRequired((Action)(() =>
            {
                if (index > this.Count)
                    return;
                this._lock.EnterWriteLock();
                try
                {
                    base.InsertItem(index, item);
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }), DispatcherPriority.DataBind);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            this._dispatcher.InvokeIfRequired((Action)(() =>
            {
                this._lock.EnterReadLock();
                int count = this.Count;
                this._lock.ExitReadLock();
                if (oldIndex >= count | newIndex >= count | oldIndex == newIndex)
                    return;
                this._lock.EnterWriteLock();
                try
                {
                    base.MoveItem(oldIndex, newIndex);
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }), DispatcherPriority.DataBind);
        }

        protected override void RemoveItem(int index)
        {
            this._dispatcher.InvokeIfRequired((Action)(() =>
            {
                if (index >= this.Count)
                    return;
                this._lock.EnterWriteLock();
                try
                {
                    base.RemoveItem(index);
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }), DispatcherPriority.DataBind);
        }

        protected override void SetItem(int index, T item)
        {
            this._dispatcher.InvokeIfRequired((Action)(() =>
            {
                this._lock.EnterWriteLock();
                try
                {
                    base.SetItem(index, item);
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }), DispatcherPriority.DataBind);
        }

        public T[] ToSyncArray()
        {
            this._lock.EnterReadLock();
            try
            {
                T[] array = new T[this.Count];
                this.CopyTo(array, 0);
                return array;
            }
            finally
            {
                this._lock.ExitReadLock();
            }
        }
    }

    public class BossGaugeWindowViewModel : DependencyObject
    {
        private static BossGaugeWindowViewModel _instance;
        public static BossGaugeWindowViewModel Instance => _instance ?? (_instance = new BossGaugeWindowViewModel());

        private SynchronizedObservableCollection<Boss> _bosses = new SynchronizedObservableCollection<Boss>();
        public SynchronizedObservableCollection<Boss> CurrentNPCs
        {
            get
            {
                if (HarrowholdMode)
                {
                    return null;
                }
                else
                {
                    return _bosses;
                }
            }
            set
            {
                if (_bosses == value) return;
                _bosses = value;
            }
        }

        public void AddOrUpdateBoss(S_BOSS_GAGE_INFO msg)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == msg.EntityId);
            if(boss == null)
            {
                if (!EntitiesManager.TryGetBossById(msg.EntityId, out Boss b)) return;
                boss = b;
                _bosses.Add(b);
            }
            boss.MaxHP = msg.MaxHP;
            boss.CurrentHP = msg.CurrentHP;
        }

        public void RemoveBoss(S_DESPAWN_NPC msg)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == msg.target);
            if (boss == null) return;
            _bosses.Remove(boss);
        }

        public bool HarrowholdMode
        {
            get => SessionManager.HarrowholdMode;            
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private void SessionManager_HhModeChanged(bool val)
        {
            RaisePropertyChanged("CurrentNPCs");
            RaisePropertyChanged("HarrowholdMode");
        }
        private bool topMost;
        public bool TopMost
        {
            get => topMost;
            set
            {
                if (topMost != value)
                {
                    topMost = value;
                    RaisePropertyChanged("TopMost");
                }
            }
        }

        public BossGaugeWindowViewModel()
        {
            SessionManager.HhModeChanged += SessionManager_HhModeChanged;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    TopMost = false;
                    TopMost = true;
                }
            };

        }

    }
}
