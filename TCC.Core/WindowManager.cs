using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TCC.ViewModels;
using TCC.Windows;
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
        public static SkillConfigWindow SkillConfigWindow;
        public static GroupAbnormalConfigWindow GroupAbnormalConfigWindow;
        public static CivilUnrestWindow CivilUnrestWindow;
        public static InfoWindow InfoWindow;
        public static FloatingButtonWindow FloatingButton;
        public static FlightDurationWindow FlightDurationWindow;
        public static LfgListWindow LfgListWindow;

        private static ContextMenu _contextMenu;

        public static NotifyIcon TrayIcon;
        public static Icon DefaultIcon;
        public static Icon ConnectedIcon;

        public static ForegroundManager ForegroundManager { get; private set; }

        //public static event PropertyChangedEventHandler ClickThruChanged;
        //public static event PropertyChangedEventHandler TccVisibilityChanged;
        //public static event PropertyChangedEventHandler TccDimChanged;


        //public static bool ClickThru
        //{
        //    get => clickThru;
        //    set
        //    {
        //        if (clickThru != value)
        //        {
        //            clickThru = value;
        //            ClickThruChanged?.Invoke(null, new PropertyChangedEventArgs("ClickThruMode"));
        //        }
        //    }
        //}
        //public static bool IsTccVisible
        //{
        //    get
        //    {
        //        if (SessionManager.Logged && !SessionManager.LoadingScreen && IsFocused)
        //        {
        //            isTccVisible = true;
        //            return isTccVisible;
        //        }
        //        else
        //        {
        //            isTccVisible = false || App.Debug;
        //            return isTccVisible;
        //        }
        //    }
        //    set
        //    {
        //        if (isTccVisible != value)
        //        {
        //            isTccVisible = value;
        //            NotifyVisibilityChanged();
        //        }
        //    }
        //}
        //public static bool IsFocused
        //{
        //    get => isFocused;
        //    set
        //    {
        //        if (!FocusManager.Running) return;
        //        //if (isFocused == value)
        //        //{
        //        //    //if(focusCount > 3)
        //        //    //{
        //        //    //    return;
        //        //    //}
        //        //    return;
        //        //}
        //        isFocused = value;
        //        //if (isFocused)
        //        //{
        //        //    focusCount++;
        //        //}
        //        //else
        //        //{
        //        //    focusCount = 0;
        //        //}
        //        NotifyVisibilityChanged();
        //    }
        //}
        //public static bool SkillsEnded
        //{
        //    get => skillsEnded;
        //    set
        //    {
        //        if (value == false)
        //        {
        //            _undimTimer.Stop();
        //            _undimTimer.Start();
        //        }
        //        if (skillsEnded == value) return;
        //        skillsEnded = value;
        //        CombatChanged?.Invoke();
        //        NotifyDimChanged();
        //    }
        //}
        //public static bool IsTccDim
        //{
        //    get => SkillsEnded && !SessionManager.Encounter; // add more conditions here if needed
        //}

        public static void Init()
        {
            ForegroundManager = new ForegroundManager();
            FocusManager.Init();
            LoadWindows();
            FloatingButton = new FloatingButtonWindow();
            FloatingButton.Show();
            _contextMenu = new ContextMenu();
            DefaultIcon = new Icon(Application.GetResourceStream(new Uri("resources/tcc-logo.ico", UriKind.Relative)).Stream);
            ConnectedIcon = new Icon(Application.GetResourceStream(new Uri("resources/tcc-logo-on.ico", UriKind.Relative)).Stream);
            TrayIcon = new NotifyIcon()
            {
                Icon = DefaultIcon,
                Visible = true
            };
            TrayIcon.MouseDown += NI_MouseDown;
            TrayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            TrayIcon.Text = $"TCC v{v.Major}.{v.Minor}.{v.Build}";
            var CloseButton = new MenuItem() { Header = "Close" };

            CloseButton.Click += (s, ev) => App.CloseApp();
            _contextMenu.Items.Add(CloseButton);

            //_undimTimer.Elapsed += _undimTimer_Elapsed;

            SettingsWindow = new SettingsWindow();

            if (Settings.UseHotkeys) KeyboardHook.Instance.RegisterKeyboardHook();
            //TccWindow.RecreateWindow += TccWindow_RecreateWindow;
            FocusManager.FocusTimer.Start();

        }

        //private static void TccWindow_RecreateWindow(TccWindow obj)
        //{
        //    if (obj is CooldownWindow) CooldownWindow = new CooldownWindow();
        //    if (obj is GroupWindow) GroupWindow = new GroupWindow();
        //    if (obj is BossWindow) BossWindow = new BossWindow();
        //    if (obj is BuffWindow) BuffWindow = new BuffWindow();
        //    if (obj is CharacterWindow) CharacterWindow = new CharacterWindow();
        //    if (obj is ClassWindow) ClassWindow = new ClassWindow();
        //    if (obj is ChatWindow) ChatWindowManager.Instance.InitWindows();
        //}

        //public static void NotifyDimChanged()
        //{
        //    TccDimChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsTccDim)));
        //}
        //public static void NotifyVisibilityChanged()
        //{
        //    TccVisibilityChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsTccVisible)));
        //}
        //public static void RefreshDim()
        //{
        //    SkillsEnded = false;
        //    Task.Delay(100).ContinueWith(t => SkillsEnded = true);
        //}
        public static void Dispose()
        {
            FocusManager.FocusTimer.Stop();
            TrayIcon?.Dispose();


            foreach (Window w in Application.Current.Windows)
            {
                try { w.Close(); } catch { }
            }

            try { CharacterWindow.CloseWindowSafe(); } catch { }
            try { CooldownWindow.CloseWindowSafe(); } catch { }
            try { GroupWindow.CloseWindowSafe(); } catch { }
            try { BossWindow.CloseWindowSafe(); } catch { }
            try { BuffWindow.CloseWindowSafe(); } catch { }
            try { InfoWindow.Close(); } catch { }
            //try { ChatWindow.CloseWindowSafe(); } catch { }
            ChatWindowManager.Instance.CloseAllWindows();
            try { ClassWindow.CloseWindowSafe(); } catch { }
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
            ChatWindowManager.Instance.InitWindows();
            GroupWindow = new GroupWindow();
            CooldownWindow = new CooldownWindow();
            BossWindow = new BossWindow();
            BuffWindow = new BuffWindow();
            CharacterWindow = new CharacterWindow();
            ClassWindow = new ClassWindow();
            InfoWindow = new InfoWindow();
            FlightDurationWindow = new FlightDurationWindow();
            LfgListWindow = new LfgListWindow();
            SkillConfigWindow = new SkillConfigWindow();
            GroupAbnormalConfigWindow = new GroupAbnormalConfigWindow();
            CivilUnrestWindow = new CivilUnrestWindow();
            //GroupWindow.Show();
            //CooldownWindow.Show();
            //BossWindow.Show();
            //BuffWindow.Show();
            //CharacterWindow.Show();
            //ClassWindow.Show();
        }
        //private static void LoadCharWindow()
        //{
        //    var charWindowThread = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        CharacterWindow = new CharacterWindow();
        //        //CharacterWindow.AllowsTransparency = Settings.CharacterWindowSettings.AllowTransparency;

        //        CharacterWindow.Show();
        //        waiting = false;
        //        Dispatcher.Run();
        //    }));
        //    charWindowThread.Name = "Character window thread";
        //    charWindowThread.SetApartmentState(ApartmentState.STA);
        //    charWindowThread.Start();
        //    Debug.WriteLine("Char window loaded");
        //}
        //private static void LoadInfoWindow()
        //{
        //    var infoWindowThread = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        InfoWindow = new InfoWindow();
        //        waiting = false;
        //        Dispatcher.Run();
        //    }));
        //    infoWindowThread.Name = "Info window thread";
        //    infoWindowThread.SetApartmentState(ApartmentState.STA);
        //    infoWindowThread.Start();
        //    Debug.WriteLine("Info window loaded");
        //}
        //private static void LoadCooldownWindow()
        //{
        //    var cooldownWindowThread = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        CooldownWindow = new CooldownWindow();
        //        //CooldownWindow.AllowsTransparency = Settings.CooldownWindowSettings.AllowTransparency;

        //        CooldownWindow.Show();
        //        waiting = false;
        //        Dispatcher.Run();
        //    }));
        //    cooldownWindowThread.Name = "Cooldown bar thread";
        //    cooldownWindowThread.SetApartmentState(ApartmentState.STA);
        //    cooldownWindowThread.Start();
        //    Debug.WriteLine("Cd window loaded");


        //}
        //private static void LoadBossGaugeWindow()
        //{

        //    var bossGaugeThread = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        BossWindow = new BossWindow();

        //        //BossWindow.AllowsTransparency = Settings.BossWindowSettings.AllowTransparency;
        //        BossWindow.Show();
        //        waiting = false;

        //        Dispatcher.Run();
        //    }));
        //    bossGaugeThread.Name = "Boss gauge thread";
        //    bossGaugeThread.SetApartmentState(ApartmentState.STA);
        //    bossGaugeThread.Start();
        //    Debug.WriteLine("Boss window loaded");

        //}
        //private static void LoadBuffBarWindow()
        //{
        //    var buffBarThread = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        BuffWindow = new BuffWindow();
        //        BuffBarWindowViewModel.Instance.Player = new Data.Player();
        //        BuffWindow.Show();
        //        waiting = false;

        //        Dispatcher.Run();
        //    }));
        //    buffBarThread.Name = "Buff bar thread";
        //    buffBarThread.SetApartmentState(ApartmentState.STA);
        //    buffBarThread.Start();
        //    Debug.WriteLine("Buff window loaded");


        //}
        //private static void LoadGroupWindow()
        //{
        //    var groupWindowThread = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        GroupWindow = new GroupWindow();
        //        GroupWindow.Show();
        //        waiting = false;

        //        Dispatcher.Run();
        //    }));
        //    groupWindowThread.Name = "Group window thread";
        //    groupWindowThread.SetApartmentState(ApartmentState.STA);
        //    groupWindowThread.Start();
        //    Debug.WriteLine("Group window loaded");

        //}
        //private static void LoadChatWindow()
        //{
        //    var chatWindowThread = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        waiting = false;

        //        Dispatcher.Run();
        //    }));
        //    chatWindowThread.Name = "Chat thread";
        //    chatWindowThread.SetApartmentState(ApartmentState.STA);
        //    chatWindowThread.Start();
        //    Debug.WriteLine("Chat window loaded");

        //}
        //private static void LoadClassWindow()
        //{
        //    var t = new Thread(new ThreadStart(() =>
        //    {
        //        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        //        ClassWindow = new ClassWindow();
        //        ClassWindow.Closed += (s, ev) => ClassWindow.Dispatcher.InvokeShutdown();
        //        ClassWindow.Show();
        //        waiting = false;

        //        Dispatcher.Run();
        //    }));
        //    t.Name = "Class bar thread";
        //    t.SetApartmentState(ApartmentState.STA);
        //    t.Start();
        //    Debug.WriteLine("Class window loaded");


        //}
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
            Settings.CooldownWindowSettings.MakePositionsGlobal();
            Settings.ClassWindowSettings.MakePositionsGlobal();
            Settings.CharacterWindowSettings.MakePositionsGlobal();
            Settings.GroupWindowSettings.MakePositionsGlobal();
            Settings.BuffWindowSettings.MakePositionsGlobal();
            Settings.BossWindowSettings.MakePositionsGlobal();

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
