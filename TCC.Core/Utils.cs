using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TCC
{
    public static class Utils
    {
        static MemoryStream ms;

        public static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            //using (MemoryStream memory = new MemoryStream())
            //{
            ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = ms;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            ms.Flush();
            ms.Close();
            ms.Dispose();
            ms = null;
            return bitmapimage;
            //}
        }

        public static Point GetRelativePoint(double x, double y, double cx, double cy)
        {
            return new Point(x - cx, y - cy);
        }

        public static string ClassEnumToString(Class c)
        {
            switch (c)
            {
                case Class.Warrior:
                    return "Warrior";
                case Class.Lancer:
                    return "Lancer";
                case Class.Slayer:
                    return "Slayer";
                case Class.Berserker:
                    return "Berserker";
                case Class.Sorcerer:
                    return "Sorcerer";
                case Class.Archer:
                    return "Archer";
                case Class.Priest:
                    return "Priest";
                case Class.Elementalist:
                    return "Mystic";
                case Class.Soulless:
                    return "Reaper";
                case Class.Engineer:
                    return "Gunner";
                case Class.Fighter:
                    return "Brawler";
                case Class.Assassin:
                    return "Ninja";
                case Class.Glaiver:
                    return "Valkyrie";
                default:
                    return "";
            }
        }

        public static string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }
    }

    public class DependencyPropertyWatcher<T> : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(DependencyPropertyWatcher<T>),
                new PropertyMetadata(null, OnPropertyChanged));

        public event EventHandler PropertyChanged;

        public DependencyPropertyWatcher(DependencyObject target, string propertyPath)
        {
            this.Target = target;
            BindingOperations.SetBinding(
                this,
                ValueProperty,
                new Binding() { Source = target, Path = new PropertyPath(propertyPath), Mode = BindingMode.OneWay });
        }

        public DependencyObject Target { get; private set; }

        public T Value
        {
            get { return (T)this.GetValue(ValueProperty); }
        }

        public static void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DependencyPropertyWatcher<T> source = (DependencyPropertyWatcher<T>)sender;

            if (source.PropertyChanged != null)
            {
                source.PropertyChanged(source, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            this.ClearValue(ValueProperty);
        }
    }
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
        protected Dispatcher _dispatcher;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string v)
        {
            _dispatcher.InvokeIfRequired(() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v)), DispatcherPriority.DataBind);
        }
    }
    public class SynchronizedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher;
        private readonly ReaderWriterLockSlim _lock;

        public SynchronizedObservableCollection()
        {
            this._dispatcher = Dispatcher.CurrentDispatcher;
            this._lock = new ReaderWriterLockSlim();
        }
        public SynchronizedObservableCollection(Dispatcher d)
        {
            this._dispatcher = d;
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
}
