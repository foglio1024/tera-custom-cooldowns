using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Win32;
using TCC.Controls;
using TCC.Data.Pc;
using TCC.Settings;
using TCC.Utilities.Extensions;
using TCC.ViewModels;
using TCC.Windows;
using TCC.Windows.Widgets;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using Size = System.Windows.Size;

namespace TCC
{
    public static class WindowManager
    {
        //private static bool clickThru;
        //private static bool isTccVisible;
        //private static bool isFocused;
        //private static bool skillsEnded = true;
        //private static int focusCount;
        //private static bool waiting;
        //private static Timer _undimTimer = new Timer(5000);

        //private static List<Delegate> WindowLoadingDelegates = new List<Delegate>
        //{
        //    new Action(LoadGroupWindow),
        //    new Action(LoadChatWindow),
        //    new Action(LoadCooldownWindow),
        //    new Action(LoadBossGaugeWindow),
        //    new Action(LoadBuffBarWindow),
        //    new Action(LoadCharWindow),
        //    new Action(LoadClassWindow),
        //    new Action(LoadInfoWindow),
        //};
        public static Size ScreenSize;

        public static CooldownWindow CooldownWindow;
        public static CharacterWindow CharacterWindow;
        public static BossWindow BossWindow;
        public static BuffWindow BuffWindow;
        public static GroupWindow GroupWindow;
        public static ClassWindow ClassWindow;
        public static SettingsWindow SettingsWindow;
        public static CivilUnrestWindow CivilUnrestWindow;
        //public static Dashboard Dashboard;
        //public static LfgListWindow LfgListWindow;
        private static Dashboard _dashboard;
        private static LfgListWindow _lfgWindow;


        public static Dashboard Dashboard => _dashboard ?? (_dashboard = new Dashboard());
        public static LfgListWindow LfgListWindow => _lfgWindow ?? (_lfgWindow = new LfgListWindow());

        public static FloatingButtonWindow FloatingButton;
        public static FlightDurationWindow FlightDurationWindow;
        public static SkillConfigWindow SkillConfigWindow;


        public static ConcurrentDictionary<int, Dispatcher> RunningDispatchers;

        private static ContextMenu _contextMenu;

        public static NotifyIcon TrayIcon;
        public static Icon DefaultIcon;
        public static Icon ConnectedIcon;

        public static ForegroundManager ForegroundManager { get; private set; }

        private static void PrintDispatcher()
        {
            Console.WriteLine("----------------------");
            foreach (var keyValuePair in RunningDispatchers)
            {
                var d = keyValuePair.Value;
                Console.WriteLine($"{d.Thread.Name} alive: {d.Thread.IsAlive} bg: {d.Thread.IsBackground}");
            }
        }

        private static System.Timers.Timer t;
        public static void UpdateScreenCorrection()
        {
            if (ScreenSize.IsEqual(SettingsHolder.LastScreenSize)) return;
            var wFac = SettingsHolder.LastScreenSize.Width / ScreenSize.Width;
            var hFac = SettingsHolder.LastScreenSize.Height / ScreenSize.Height;
            var sc = new Size(wFac, hFac);
            SettingsHolder.LastScreenSize = ScreenSize;
            ApplyScreenCorrection(sc);
            if (!App.Loading) SettingsWriter.Save();
        }

        private static void ApplyScreenCorrection(Size sc)
        {
            var list = new List<WindowSettings>
            {
                CooldownWindow.WindowSettings,
                ClassWindow.WindowSettings,
                CharacterWindow.WindowSettings,
                GroupWindow.WindowSettings,
                BuffWindow.WindowSettings,
                BossWindow.WindowSettings
            };

            list.ForEach(s => { s.ApplyScreenCorrection(sc); });

        }

        public static void Init()
        {
            ForegroundManager = new ForegroundManager();
            ScreenSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            FocusManager.Init();
            LoadWindows();
            UpdateScreenCorrection();
            _contextMenu = new ContextMenu();
            var defaultIconStream = Application.GetResourceStream(new Uri("resources/tcc-logo.ico", UriKind.Relative))?.Stream;
            if (defaultIconStream != null) DefaultIcon = new Icon(defaultIconStream);
            var connectedIconStream = Application.GetResourceStream(new Uri("resources/tcc-logo-on.ico", UriKind.Relative))?.Stream;
            if (connectedIconStream != null) ConnectedIcon = new Icon(connectedIconStream);
            TrayIcon = new NotifyIcon()
            {
                Icon = DefaultIcon,
                Visible = true
            };
            TrayIcon.MouseDown += NI_MouseDown;
            TrayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            TrayIcon.Text = $"TCC v{v.Major}.{v.Minor}.{v.Build}";

            _contextMenu.Items.Add(new MenuItem() { Header = "Dashboard", Command = new RelayCommand(o => Dashboard.ShowWindow()) });
            _contextMenu.Items.Add(new MenuItem() { Header = "Settings", Command = new RelayCommand(o => SettingsWindow.ShowWindow()) });
            _contextMenu.Items.Add(new MenuItem()
            {
                Header = "Close",
                Command = new RelayCommand(o =>
                {
                    _contextMenu.Closed += (_, __) => App.Close();
                    _contextMenu.IsOpen = false;
                })
            });

            SettingsWindow = new SettingsWindow();

            if (SettingsHolder.UseHotkeys) KeyboardHook.Instance.RegisterKeyboardHook();

            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;
            //t = new System.Timers.Timer();
            //t.Interval = 1000;
            //t.Elapsed += (_, __) => PrintDispatcher();
            //t.Start();
        }

        private static void SystemEventsOnDisplaySettingsChanged(object sender, EventArgs e)
        {
            ScreenSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            UpdateScreenCorrection();
            ReloadPositions();
        }

        public static void CloseWindow(Type type)
        {
            Application.Current.Windows.ToList().ForEach(w =>
            {
                if (w.GetType() == type) w.Close();
            });
        }

        public static bool IsWindowOpen(Type type)
        {
            return Application.Current.Windows.ToList().Any(w =>
            w.GetType() == type && w.IsVisible);
        }

        internal static void RemoveEmptyChatWindows()
        {
            App.BaseDispatcher.BeginInvoke(new Action(() =>
            {
                foreach (ChatWindow w in Application.Current.Windows.ToList().Where(x => x is ChatWindow c && c.VM.TabVMs.Count == 0))
                {
                    ChatWindowManager.Instance.ChatWindows.Remove(w);
                    w.Close();
                }

                if (FocusManager.ForceFocused) FocusManager.ForceFocused = false;

            }), DispatcherPriority.Background);
        }

        public static void Dispose()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;

            App.BaseDispatcher.Invoke(() =>
            {
                FocusManager.Dispose();
                TrayIcon?.Dispose();

                foreach (Window w in Application.Current.Windows)
                {
                    if (w is TccWidget) continue;
                    try { w.Close(); } catch { }
                }
            });

            ChatWindowManager.Instance.CloseAllWindows();

            try { CharacterWindow.CloseWindowSafe(); } catch { }
            try { CooldownWindow.CloseWindowSafe(); } catch { }
            try { GroupWindow.CloseWindowSafe(); } catch { }
            try { BossWindow.CloseWindowSafe(); } catch { }
            try { BuffWindow.CloseWindowSafe(); } catch { }
            try { ClassWindow.CloseWindowSafe(); } catch { }

            if (RunningDispatchers == null) return;
            var times = 50;
            while (times > 0)
            {
                if (RunningDispatchers.Count == 0) break;
                Log.CW("Waiting all dispatcher to shutdown...");
                Thread.Sleep(100);
                times--;
            }

        }
        private static void LoadWindows()
        {
            //waiting = true;
            //foreach (var del in WindowLoadingDelegates)
            //{
            //    waiting = true;
            //    del.DynamicInvoke();
            //    while (waiting) { }
            //}
            RunningDispatchers = new ConcurrentDictionary<int, Dispatcher>();
            LoadCooldownWindow();
            LoadClassWindow();
            LoadGroupWindow();
            LoadNpcWindow();
            LoadCharWindow();
            LoadBuffBarWindow();

            FlightDurationWindow = new FlightDurationWindow();
            if (FlightDurationWindow.WindowSettings.Enabled) FlightDurationWindow.Show();

            CivilUnrestWindow = new CivilUnrestWindow();
            if (CivilUnrestWindow.WindowSettings.Enabled) CivilUnrestWindow.Show();

            FloatingButton = new FloatingButtonWindow();
            if (FloatingButton.WindowSettings.Enabled) FloatingButton.Show();

            //LfgListWindow = new LfgListWindow();
            //Dashboard = new Dashboard();

            ChatWindowManager.Instance.InitWindows();

        }
        public static bool ChatInitalized = false;

        private static void AddDispatcher(int threadId, Dispatcher d)
        {
            RunningDispatchers[threadId] = d;
        }
        private static void RemoveDispatcher(int threadId)
        {
            RunningDispatchers.TryRemove(threadId, out var _);
        }

        private static void LoadCooldownWindow()
        {
            var cooldownWindowThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                CooldownWindow = new CooldownWindow();
                if (CooldownWindow.WindowSettings.Enabled) CooldownWindow.Show();
                AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
                Dispatcher.Run();
                RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
            })
            { Name = "Cdwn" };
            cooldownWindowThread.SetApartmentState(ApartmentState.STA);
            cooldownWindowThread.Start();
        }
        private static void LoadClassWindow()
        {
            var classWindowThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                ClassWindow = new ClassWindow();
                if (ClassWindow.WindowSettings.Enabled) ClassWindow.Show();
                AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
                Dispatcher.Run();
                RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
            })
            { Name = "Class" };
            classWindowThread.SetApartmentState(ApartmentState.STA);
            classWindowThread.Start();
        }
        private static void LoadCharWindow()
        {
            var charWindowThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                SessionManager.CurrentPlayer = new Player();
                CharacterWindow = new CharacterWindow();
                if (CharacterWindow.WindowSettings.Enabled) CharacterWindow.Show();
                AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
                Dispatcher.Run();
                RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
            })
            { Name = "Char" };
            charWindowThread.SetApartmentState(ApartmentState.STA);
            charWindowThread.Start();
        }
        private static void LoadNpcWindow()
        {
            var bossGaugeThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BossWindow = new BossWindow();
                if (BossWindow.WindowSettings.Enabled) BossWindow.Show();
                AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
                Dispatcher.Run();
                RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
            })
            { Name = "Boss" };
            bossGaugeThread.SetApartmentState(ApartmentState.STA);
            bossGaugeThread.Start();
        }
        private static void LoadBuffBarWindow()
        {
            var buffBarThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BuffWindow = new BuffWindow();
                if (BuffWindow.WindowSettings.Enabled) BuffWindow.Show();
                AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
                Dispatcher.Run();
                RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
            })
            { Name = "Buff" };
            buffBarThread.SetApartmentState(ApartmentState.STA);
            buffBarThread.Start();
        }
        private static void LoadGroupWindow()
        {
            var groupWindowThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                GroupWindow = new GroupWindow();
                if (GroupWindow.WindowSettings.Enabled) GroupWindow.Show();
                AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
                Dispatcher.Run();
                RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
            })
            { Name = "Group" };
            groupWindowThread.SetApartmentState(ApartmentState.STA);
            groupWindowThread.Start();
        }

        private static void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SettingsWindow == null)
            {
                SettingsWindow = new SettingsWindow
                {
                    Name = "Settings"
                };
            }
            SettingsWindow.ShowWindow();
        }
        private static void NI_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    _contextMenu.IsOpen = true;
                    break;
                case MouseButtons.Left:
                    _contextMenu.IsOpen = false;
                    break;
            }
        }

        public static void ReloadPositions()
        {
            CooldownWindow.ReloadPosition();
            ClassWindow.ReloadPosition();
            CharacterWindow.ReloadPosition();
            GroupWindow.ReloadPosition();
            BuffWindow.ReloadPosition();
            BossWindow.ReloadPosition();
        }

        public static void MakeGlobal()
        {
            SettingsHolder.CooldownWindowSettings.MakePositionsGlobal();
            SettingsHolder.ClassWindowSettings.MakePositionsGlobal();
            SettingsHolder.CharacterWindowSettings.MakePositionsGlobal();
            SettingsHolder.GroupWindowSettings.MakePositionsGlobal();
            SettingsHolder.BuffWindowSettings.MakePositionsGlobal();
            SettingsHolder.BossWindowSettings.MakePositionsGlobal();

            SettingsWriter.Save();
        }

        public static void ResetToCenter()
        {
            CooldownWindow.ResetToCenter();
            ClassWindow.ResetToCenter();
            CharacterWindow.ResetToCenter();
            GroupWindow.ResetToCenter();
            BuffWindow.ResetToCenter();
            BossWindow.ResetToCenter();
            FlightDurationWindow.ResetToCenter();
        }
    }
}
