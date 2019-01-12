using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Settings;
using TCC.ViewModels;
using TCC.Windows;
using TCC.Windows.Widgets;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

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

        public static CooldownWindow CooldownWindow;
        public static CharacterWindow CharacterWindow;
        public static BossWindow BossWindow;
        public static BuffWindow BuffWindow;
        public static GroupWindow GroupWindow;
        public static ClassWindow ClassWindow;
        public static SettingsWindow SettingsWindow;
        public static CivilUnrestWindow CivilUnrestWindow;
        public static Dashboard Dashboard;
        public static FloatingButtonWindow FloatingButton;
        public static FlightDurationWindow FlightDurationWindow;
        public static LfgListWindow LfgListWindow;

        private static ContextMenu _contextMenu;

        public static NotifyIcon TrayIcon;
        public static Icon DefaultIcon;
        public static Icon ConnectedIcon;

        public static ForegroundManager ForegroundManager { get; private set; }

        public static void Init()
        {
            ForegroundManager = new ForegroundManager();
            FocusManager.Init();
            LoadWindows();
            _contextMenu = new ContextMenu();
            DefaultIcon = new Icon(Application.GetResourceStream(new Uri("resources/tcc-logo.ico", UriKind.Relative))?.Stream);
            ConnectedIcon = new Icon(Application.GetResourceStream(new Uri("resources/tcc-logo-on.ico", UriKind.Relative))?.Stream);
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
            _contextMenu.Items.Add(new MenuItem() { Header = "Close", Command = new RelayCommand(o => App.CloseApp()) });

            SettingsWindow = new SettingsWindow();

            if (SettingsHolder.UseHotkeys) KeyboardHook.Instance.RegisterKeyboardHook();
        }

        public static void CloseWindow(Type type)
        {
            App.Current.Windows.ToList().ForEach(w =>
            {
                if (w.GetType() == type) w.Close();
            });
        }

        public static bool IsWindowOpen(Type type)
        {
            return App.Current.Windows.ToList().Any(w => 
            w.GetType() == type && w.IsVisible);
        }

        internal static void RemoveEmptyChatWindows()
        {
            App.BaseDispatcher.BeginInvoke(new Action(() =>
            {
                foreach (ChatWindow w in App.Current.Windows.ToList().Where(x => x is ChatWindow c && c.VM.TabVMs.Count == 0))
                {
                    ChatWindowManager.Instance.ChatWindows.Remove(w);
                    w.Close();
                }

                if (FocusManager.ForceFocused) FocusManager.ForceFocused = false;

            }), DispatcherPriority.Background);
        }

        public static void Dispose()
        {
            FocusManager.Dispose();
            TrayIcon?.Dispose();

            try { CharacterWindow.CloseWindowSafe(); } catch { }
            try { CooldownWindow.CloseWindowSafe(); } catch { }
            try { GroupWindow.CloseWindowSafe(); } catch { }
            try { BossWindow.CloseWindowSafe(); } catch { }
            try { BuffWindow.CloseWindowSafe(); } catch { }
            try { Dashboard.Close(); } catch { }
            try { ClassWindow.CloseWindowSafe(); } catch { }

            ChatWindowManager.Instance.CloseAllWindows();


            foreach (Window w in Application.Current.Windows)
            {
                try { w.Close(); } catch { }
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

            LfgListWindow = new LfgListWindow();
            Dashboard = new Dashboard();

            ChatWindowManager.Instance.InitWindows();

        }
        public static bool _chatInitalized = false;
        private static void LoadCooldownWindow()
        {
            var cooldownWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                CooldownWindow = new CooldownWindow();
                if (CooldownWindow.WindowSettings.Enabled) CooldownWindow.Show();
                Dispatcher.Run();
            }));
            cooldownWindowThread.Name = "Cdwn";
            cooldownWindowThread.SetApartmentState(ApartmentState.STA);
            cooldownWindowThread.Start();
        }
        private static void LoadClassWindow()
        {
            var classWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                ClassWindow = new ClassWindow();
                if (ClassWindow.WindowSettings.Enabled) ClassWindow.Show();
                Dispatcher.Run();
            }));
            classWindowThread.Name = "Class";

            classWindowThread.SetApartmentState(ApartmentState.STA);
            classWindowThread.Start();
        }
        private static void LoadCharWindow()
        {
            var charWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                CharacterWindow = new CharacterWindow();
                if (CharacterWindow.WindowSettings.Enabled) CharacterWindow.Show();
                Dispatcher.Run();
            }));
            charWindowThread.Name = "Char";
            charWindowThread.SetApartmentState(ApartmentState.STA);
            charWindowThread.Start();
        }
        private static void LoadNpcWindow()
        {
            var bossGaugeThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BossWindow = new BossWindow();
                if (BossWindow.WindowSettings.Enabled) BossWindow.Show();
                Dispatcher.Run();
            }));
            bossGaugeThread.Name = "Boss";
            bossGaugeThread.SetApartmentState(ApartmentState.STA);
            bossGaugeThread.Start();
        }
        private static void LoadBuffBarWindow()
        {
            var buffBarThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BuffWindow = new BuffWindow();
                if (BuffWindow.WindowSettings.Enabled) BuffWindow.Show();
                Dispatcher.Run();
            }));
            buffBarThread.Name = "Buff";
            buffBarThread.SetApartmentState(ApartmentState.STA);
            buffBarThread.Start();
        }
        private static void LoadGroupWindow()
        {
            var groupWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                GroupWindow = new GroupWindow();
                if (GroupWindow.WindowSettings.Enabled) GroupWindow.Show();

                Dispatcher.Run();
            }));
            groupWindowThread.Name = "Group";
            groupWindowThread.SetApartmentState(ApartmentState.STA);
            groupWindowThread.Start();
        }

        private static void TrayIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (SettingsWindow == null)
            {
                SettingsWindow = new SettingsWindow()
                {
                    Name = "Settings"
                };
            }
            SettingsWindow.ShowWindow();
        }
        private static void NI_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                _contextMenu.IsOpen = true;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _contextMenu.IsOpen = false;
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
