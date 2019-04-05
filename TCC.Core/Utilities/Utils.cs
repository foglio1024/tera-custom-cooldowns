using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TCC.Annotations;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Utilities.Extensions;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace TCC
{
    public static class Utils
    {

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                ms.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = ms;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                ms.Flush();
                ms.Close();
                ms.Dispose();
                return bitmapimage;
            }
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
                case Class.Mystic:
                    return "Mystic";
                case Class.Reaper:
                    return "Reaper";
                case Class.Gunner:
                    return "Gunner";
                case Class.Brawler:
                    return "Brawler";
                case Class.Ninja:
                    return "Ninja";
                case Class.Valkyrie:
                    return "Valkyrie";
                case Class.Common:
                    return "All classes";
                default:
                    return "";
            }
        }


        public static bool IsPhase1Dragon(uint zoneId, uint templateId)
        {
            return zoneId == 950 && templateId >= 1100 && templateId <= 1103;
        }

        public static bool IsGuildTower(uint zoneId, uint templateId)
        {
            return zoneId == 152 && templateId == 5001;
        }

        public static List<ChatChannelOnOff> GetEnabledChannelsList()
        {
            var ch = ListFromEnum<ChatChannel>();
            var result = new List<ChatChannelOnOff>();
            foreach (var c in ch)
            {
                result.Add(new ChatChannelOnOff(c));
            }

            return result;
        }

        public static List<T> ListFromEnum<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static double FactorToAngle(double value, double multiplier = 1)
        {
            return value * (359.9 / multiplier);
        }

        public static T FindVisualParent<T>(DependencyObject sender) where T : DependencyObject
        {
            if (sender == null)
            {
                return null;
            }
            else if (VisualTreeHelper.GetParent(sender) is T)
            {
                return VisualTreeHelper.GetParent(sender) as T;
            }
            else
            {
                var parent = VisualTreeHelper.GetParent(sender);
                return FindVisualParent<T>(parent);
            }
        }

        public static T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject child = null;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child.GetType() == typeof(T))
                {
                    break;
                }
                else
                {
                    child = GetChild<T>(child);
                    if (child != null && child.GetType() == typeof(T))
                    {
                        break;
                    }
                }
            }

            return child as T;
        }

        public static Color ParseColor(string col)
        {
            if (col.StartsWith("#")) col = col.Substring(1);
            return Color.FromRgb(
                Convert.ToByte(col.Substring(0, 2), 16),
                Convert.ToByte(col.Substring(2, 2), 16),
                Convert.ToByte(col.Substring(4, 2), 16));
        }

        public static double FactorCalc(double val, double max)
        {
            return max > 0
                ? val / max > 1 ? 1 : val / max
                : 1;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static string TimeFormatter(ulong seconds)
        {
            if (seconds < 99) return seconds + "";
            if (seconds < 99 * 60) return seconds / 60 + "m";
            if (seconds < 99 * 60 * 60) return seconds / (60 * 60) + "h";
            return seconds / (60 * 60 * 24) + "d";
        }

        public static ICollectionView InitView(Predicate<object> filter, IEnumerable source, IEnumerable<SortDescription> sortDescr)
        {
            var view = new CollectionViewSource { Source = source }.View;
            view.Filter = filter;
            foreach (var sd in sortDescr)
            {
                view.SortDescriptions.Add(sd);
            }
            return view;
        }

        internal static RegionEnum RegionEnumFromLanguage(string language)
        {
            if (Enum.TryParse<RegionEnum>(language, out var res))
            {
                return res;
            }
            else if (language.StartsWith("EU")) return RegionEnum.EU;
            else if (language.StartsWith("KR")) return RegionEnum.KR;
            else if (language == "THA" || language == "SE") return RegionEnum.THA;
            else return RegionEnum.EU;
        }

        public static ICollectionViewLiveShaping InitLiveView<T>(Predicate<object> predicate, IEnumerable<T> source,
            string[] filters, SortDescription[] sortFilters)
        {
            var cv = new CollectionViewSource { Source = source }.View;
            cv.Filter = predicate;
            if (!(cv is ICollectionViewLiveShaping liveView)) return null;
            if (!liveView.CanChangeLiveFiltering) return null;
            if (filters.Length > 0)
            {
                foreach (var filter in filters)
                {
                    liveView.LiveFilteringProperties.Add(filter);
                }
                liveView.IsLiveFiltering = true;
            }

            if (sortFilters.Length <= 0) return liveView;

            foreach (var filter in sortFilters)
            {
                ((ICollectionView)liveView).SortDescriptions.Add(filter);
                liveView.LiveSortingProperties.Add(filter.PropertyName);
            }

            liveView.IsLiveSorting = true;

            return liveView;
        }

        public static double Factor(double value, double maxValue)
        {
            if (maxValue == 0) return 1;
            var n = value / maxValue;
            return n;
        }

        public static WebClient GetDefaultWebClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
            return wc;

        }

        public static string GenerateFileHash(string fileName)
        {
            if (!File.Exists(fileName)) return "";
            byte[] fileBuffer;
            try
            {
                fileBuffer = File.ReadAllBytes(fileName);
            }
            catch
            {
                Log.F($"Failed to check hash on file {fileName}");
                return "";
            }
            //var file = File.Open(fileName, FileMode.Open);
            //var fileBuffer = new byte[file.Length];
            //file.Read(fileBuffer, 0, (int)file.Length);
            //file.Close();
            return SHA256.Create().ComputeHash(fileBuffer).ToStringEx();

        }

        public static bool IsFileLocked(string filename, FileAccess fileAccess)
        {
            // Try to open the file with the indicated access.
            try
            {
                var fs = new FileStream(filename, FileMode.Open, fileAccess);
                fs.Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();
    }

    public static class EventUtils
    {
        public static bool EndsToday(double start, double ed, bool d)
        {
            return d ? start + ed <= 24 : start <= ed;
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
            Target = target;
            BindingOperations.SetBinding(
                this,
                ValueProperty,
                new Binding() { Source = target, Path = new PropertyPath(propertyPath), Mode = BindingMode.OneWay });
        }

        public DependencyObject Target { [UsedImplicitly] get; private set; }

        public T Value => (T)GetValue(ValueProperty);

        public static void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var source = (DependencyPropertyWatcher<T>)sender;

            source.PropertyChanged?.Invoke(source, EventArgs.Empty);
        }

        public void Dispose()
        {
            ClearValue(ValueProperty);
        }
    }

    public class TSPropertyChanged : INotifyPropertyChanged
    {
        protected Dispatcher Dispatcher;
        public Dispatcher GetDispatcher()
        {
            return Dispatcher;
        }
        public void SetDispatcher(Dispatcher newDispatcher)
        {
            Dispatcher = newDispatcher;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void N([CallerMemberName] string v = null)
        {
            if (Dispatcher == null) SetDispatcher(App.BaseDispatcher);
            Dispatcher.BeginInvokeIfRequired(() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v)), DispatcherPriority.DataBind);
        }

        public void ExN(string v)
        {
            N(v);
        }
    }
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

    public static class ContextMenuLeftClickBehavior
    {
        public static bool GetIsLeftClickEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLeftClickEnabledProperty);
        }

        public static void SetIsLeftClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLeftClickEnabledProperty, value);
        }

        public static readonly DependencyProperty IsLeftClickEnabledProperty = DependencyProperty.RegisterAttached(
            "IsLeftClickEnabled",
            typeof(bool),
            typeof(ContextMenuLeftClickBehavior),
            new UIPropertyMetadata(false, OnIsLeftClickEnabledChanged));

        private static void OnIsLeftClickEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = sender as UIElement;

            if (uiElement != null)
            {
                var isEnabled = e.NewValue is bool && (bool)e.NewValue;

                if (isEnabled)
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click += OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
                }
                else
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click -= OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement fe)) return;
            // if we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click
            // (it seems setting DataContext for ContextMenu is hardcoded in WPF when user right clicks on a control, although I'm not sure)
            // so we have to set up ContextMenu.DataContext manually here

            if (fe.ContextMenu == null) return;
            if (fe.ContextMenu.DataContext == null)
            {
                fe.ContextMenu.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = fe.DataContext });
            }
            fe.ContextMenu.IsOpen = true;
        }
    }
}
