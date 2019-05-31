using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;
using FoglioUtils.Extensions;

namespace TCC
{
    public class SynchronizedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher;
        private readonly ReaderWriterLockSlim _lock;
        public SynchronizedObservableCollection()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _lock = new ReaderWriterLockSlim();
            BindingOperations.EnableCollectionSynchronization(this, _lock);
        }
        public SynchronizedObservableCollection(Dispatcher d)
        {
            _dispatcher = d ?? Dispatcher.CurrentDispatcher;
            _lock = new ReaderWriterLockSlim();
            BindingOperations.EnableCollectionSynchronization(this, _lock);
        }
        protected override void ClearItems()
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                _lock.EnterWriteLock();
                try
                {
                    base.ClearItems();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }
        protected override void InsertItem(int index, T item)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                if (index > Count)
                    return;
                _lock.EnterWriteLock();
                try
                {
                    base.InsertItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                _lock.EnterReadLock();
                var count = Count;
                _lock.ExitReadLock();
                if (oldIndex >= count | newIndex >= count | oldIndex == newIndex)
                    return;
                _lock.EnterWriteLock();
                try
                {
                    base.MoveItem(oldIndex, newIndex);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }
        protected override void RemoveItem(int index)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                if (index >= Count)
                    return;
                _lock.EnterWriteLock();
                try
                {
                    base.RemoveItem(index);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }
        protected override void SetItem(int index, T item)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                _lock.EnterWriteLock();
                try
                {
                    base.SetItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }
        //public T[] ToSyncArray()
        //{
        //    _lock.EnterReadLock();
        //    try
        //    {
        //        var array = new T[Count];
        //        CopyTo(array, 0);
        //        return array;
        //    }
        //    finally
        //    {
        //        _lock.ExitReadLock();
        //    }
        //}
        public List<T> ToSyncList()
        {
            _lock.EnterReadLock();
            try
            {
                var list = new List<T>();
                list.AddRange(this);
                return list;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
