using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Win32;
using TCC.Controls;
using TCC.Data.Pc;
using TCC.Settings;
using FoglioUtils.Extensions;
using TCC.Utilities;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;
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
        /// <summary>
        /// Container class to keep reference to view models
        /// </summary>
        public static class ViewModels
        {
            private static CooldownWindowViewModel _cooldowns;
            private static CharacterWindowViewModel _character;
            private static NpcWindowViewModel _npc;
            private static BuffBarWindowViewModel _abnormal;
            private static GroupWindowViewModel _group;
            private static ClassWindowViewModel _class;
            private static CivilUnrestViewModel _civilUnrest;
            private static DashboardViewModel _dashboard;
            private static LfgListViewModel _lfg;
            private static FlightGaugeViewModel _flightGauge;

            public static CooldownWindowViewModel Cooldowns => _cooldowns ?? (_cooldowns = new CooldownWindowViewModel(App.Settings.CooldownWindowSettings));
            public static CharacterWindowViewModel Character => _character ?? (_character = new CharacterWindowViewModel(App.Settings.CharacterWindowSettings));
            public static NpcWindowViewModel NPC => _npc ?? (_npc = new NpcWindowViewModel(App.Settings.NpcWindowSettings));
            public static BuffBarWindowViewModel Abnormal => _abnormal ?? (_abnormal = new BuffBarWindowViewModel(App.Settings.BuffWindowSettings));
            private static readonly object _groupVmLock = new object();
            public static GroupWindowViewModel Group
            {
                get
                {
                    // TODO: temporary workaround
                    lock (_groupVmLock)
                    {
                        return _group ?? (_group = new GroupWindowViewModel(App.Settings.GroupWindowSettings));
                    }
                }
            }

            public static ClassWindowViewModel Class => _class ?? (_class = new ClassWindowViewModel(App.Settings.ClassWindowSettings));
            public static CivilUnrestViewModel CivilUnrest => _civilUnrest ?? (_civilUnrest = new CivilUnrestViewModel(App.Settings.CivilUnrestWindowSettings));
            public static DashboardViewModel Dashboard => _dashboard ?? (_dashboard = new DashboardViewModel(null));
            public static LfgListViewModel LFG => _lfg ?? (_lfg = new LfgListViewModel(App.Settings.LfgWindowSettings));
            public static FlightGaugeViewModel FlightGauge => _flightGauge ?? (_flightGauge = new FlightGaugeViewModel(App.Settings.FlightGaugeWindowSettings));

        }

        public static Size ScreenSize;

        public static CooldownWindow CooldownWindow;
        public static CharacterWindow CharacterWindow;
        public static BossWindow BossWindow;
        public static BuffWindow BuffWindow;
        public static GroupWindow GroupWindow;
        public static ClassWindow ClassWindow;
        public static SettingsWindow SettingsWindow;
        public static CivilUnrestWindow CivilUnrestWindow;
        private static Dashboard _dashboardWindow;
        private static LfgListWindow _lfgWindow;


        public static Dashboard DashboardWindow
        {
            get
            {
                if (_dashboardWindow != null) return _dashboardWindow;
                _dashboardWindow = new Dashboard(ViewModels.Dashboard);
                return _dashboardWindow;
            }
        }

        public static LfgListWindow LfgListWindow
        {
            get
            {
                if (_lfgWindow != null) return _lfgWindow;
                _lfgWindow = new LfgListWindow(ViewModels.LFG);
                return _lfgWindow;
            }
        }

        public static FloatingButtonWindow FloatingButton;
        public static FlightDurationWindow FlightDurationWindow;
        public static SkillConfigWindow SkillConfigWindow;


        public static ConcurrentDictionary<int, Dispatcher> RunningDispatchers;

        private static ContextMenu _contextMenu;

        public static NotifyIcon TrayIcon;
        public static Icon DefaultIcon;
        public static Icon ConnectedIcon;

        public static ForegroundManager ForegroundManager { get; set; }

        private static void PrintDispatcher()
        {
            Console.WriteLine("----------------------");
            foreach (var keyValuePair in RunningDispatchers)
            {
                var d = keyValuePair.Value;
                Console.WriteLine($"{d.Thread.Name} alive: {d.Thread.IsAlive} bg: {d.Thread.IsBackground}");
            }
        }
        private static Size GetScreenCorrection()
        {
            var wFac = App.Settings.LastScreenSize.Width / ScreenSize.Width;
            var hFac = App.Settings.LastScreenSize.Height / ScreenSize.Height;
            return new Size(wFac, hFac);
        }
        public static void UpdateScreenCorrection()
        {
            if (ScreenSize.IsEqual(App.Settings.LastScreenSize)) return;
            App.Settings.LastScreenSize = ScreenSize;
            ApplyScreenCorrection(GetScreenCorrection());
            if (!App.Loading) App.Settings.Save();
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
            ScreenSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            FocusManager.Init();
            LoadWindows();
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

            _contextMenu.Items.Add(new MenuItem() { Header = "Dashboard", Command = new RelayCommand(o => DashboardWindow.ShowWindow()) });
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

            if (App.Settings.UseHotkeys) KeyboardHook.Instance.RegisterKeyboardHook();

            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

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

            RunningDispatchers = new ConcurrentDictionary<int, Dispatcher>();
            LoadCharWindow();
            LoadCooldownWindow();
            LoadClassWindow();
            LoadGroupWindow();
            LoadNpcWindow();
            LoadBuffBarWindow();

            FlightDurationWindow = new FlightDurationWindow(ViewModels.FlightGauge);
            if (FlightDurationWindow.WindowSettings.Enabled) FlightDurationWindow.Show();

            CivilUnrestWindow = new CivilUnrestWindow(ViewModels.CivilUnrest);
            if (CivilUnrestWindow.WindowSettings.Enabled) CivilUnrestWindow.Show();

            FloatingButton = new FloatingButtonWindow();
            if (FloatingButton.WindowSettings.Enabled) FloatingButton.Show();

            _dashboardWindow = new Dashboard(ViewModels.Dashboard);
            _lfgWindow = new LfgListWindow(ViewModels.LFG);



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
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                CooldownWindow = new CooldownWindow(ViewModels.Cooldowns);
                if (CooldownWindow.WindowSettings.Enabled) CooldownWindow.Show();
                CooldownWindow.WindowSettings.ApplyScreenCorrection(GetScreenCorrection());
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
                ClassWindow = new ClassWindow(ViewModels.Class);
                if (ClassWindow.WindowSettings.Enabled) ClassWindow.Show();
                ClassWindow.WindowSettings.ApplyScreenCorrection(GetScreenCorrection());

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
                Game.Me = new Player();
                CharacterWindow = new CharacterWindow(ViewModels.Character);
                if (CharacterWindow.WindowSettings.Enabled) CharacterWindow.Show();
                CharacterWindow.WindowSettings.ApplyScreenCorrection(GetScreenCorrection());
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
                BossWindow = new BossWindow(ViewModels.NPC);
                if (BossWindow.WindowSettings.Enabled) BossWindow.Show();
                BossWindow.WindowSettings.ApplyScreenCorrection(GetScreenCorrection());
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
                BuffWindow = new BuffWindow(ViewModels.Abnormal);
                if (BuffWindow.WindowSettings.Enabled) BuffWindow.Show();
                BuffWindow.WindowSettings.ApplyScreenCorrection(GetScreenCorrection());
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
                GroupWindow = new GroupWindow(ViewModels.Group);
                if (GroupWindow.WindowSettings.Enabled) GroupWindow.Show();
                GroupWindow.WindowSettings.ApplyScreenCorrection(GetScreenCorrection());
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
            App.Settings.CooldownWindowSettings.MakePositionsGlobal();
            App.Settings.ClassWindowSettings.MakePositionsGlobal();
            App.Settings.CharacterWindowSettings.MakePositionsGlobal();
            App.Settings.GroupWindowSettings.MakePositionsGlobal();
            App.Settings.BuffWindowSettings.MakePositionsGlobal();
            App.Settings.NpcWindowSettings.MakePositionsGlobal();

            App.Settings.Save();
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
