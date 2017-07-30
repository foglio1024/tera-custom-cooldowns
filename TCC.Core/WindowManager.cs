using DamageMeter.Sniffing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public static class WindowManager
    {
        public static CooldownWindow CooldownWindow;
        public static CharacterWindow CharacterWindow;
        public static BossWindow BossWindow;
        public static BuffWindow BuffWindow;
        public static GroupWindow GroupWindow;
        public static ClassWindow ClassWindow;
        public static ChatWindow ChatWindow;
        public static SettingsWindow Settings;

        public static ContextMenu ContextMenu;

        public static System.Windows.Forms.NotifyIcon TrayIcon;
        public static Icon DefaultIcon;
        public static Icon ConnectedIcon;


        private static bool clickThru;
        public static bool ClickThru
        {
            get => clickThru;
            set
            {
                if (clickThru != value)
                {
                    clickThru = value;
                    ClickThruChanged?.Invoke(null, new PropertyChangedEventArgs("ClickThru"));
                }
            }
        }
        private static bool isTccVisible;
        public static bool IsTccVisible
        {
            get
            {
                var debug = false;
                if (SessionManager.Logged && !SessionManager.LoadingScreen && IsFocused)
                {
                    isTccVisible = true;
                    return isTccVisible;
                }
                else
                {
                    isTccVisible = false || debug;
                    return isTccVisible;
                }
            }
            set
            {
                if (isTccVisible != value)
                {
                    isTccVisible = value;
                    NotifyVisibilityChanged();
                }
            }
        }
        private static bool isFocused;
        public static bool IsFocused
        {
            get => isFocused;
            set
            {

                if (isFocused == value)
                {
                    if(focusCount > 3)
                    {
                        return;
                    }
                }
                isFocused = value;
                if (isFocused)
                {
                    focusCount++;
                }
                else
                {
                    focusCount = 0;
                }
                NotifyVisibilityChanged();             
            }
        }
        static int focusCount;
        static System.Timers.Timer _undimTimer;

        private static bool skillsEnded = true;
        public static bool SkillsEnded
        {
            get => skillsEnded;
            set
            {
                if (value == false)
                {
                    _undimTimer.Stop();
                    _undimTimer.Start();
                }
                if (skillsEnded == value) return;
                skillsEnded = value;
                NotifyDimChanged();
            }
        }

        public static bool IsTccDim
        {
            get => SkillsEnded && !SessionManager.Encounter; // add more conditions here if needed
        }

        public static void NotifyDimChanged()
        {
            TccDimChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsTccDim)));
        }
        public static void NotifyVisibilityChanged()
        {
            TccVisibilityChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsTccVisible)));
        }

        public static event PropertyChangedEventHandler ClickThruChanged;
        public static event PropertyChangedEventHandler TccVisibilityChanged;
        public static event PropertyChangedEventHandler TccDimChanged;

        public static Visibility StaminaGaugeVisibility;
        public static double StaminaGaugeTop;
        public static double StaminaGaugeLeft;
        public static void RefreshDim()
        {
            SkillsEnded = false;
            Task.Delay(100).ContinueWith(t => SkillsEnded = true);
        }
        static List<Delegate> WindowLoadingDelegates = new List<Delegate>
        {
            new Action(LoadCooldownWindow),
            new Action(LoadBossGaugeWindow),
            new Action(LoadBuffBarWindow),
            new Action(LoadGroupWindow),
            new Action(LoadChatWindow),
            new Action(LoadCharWindow),
            new Action(LoadClassWindow)
        };
        public static void LoadCharWindow()
        {
            var charWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                CharacterWindow = new CharacterWindow();
                CharacterWindow.AllowsTransparency = SettingsManager.CharacterWindowSettings.AllowTransparency;

                CharacterWindowViewModel.Instance.Player = new Data.Player();
                CharacterWindow.Show();
                waiting = false;
                Dispatcher.Run();
            }));
            charWindowThread.Name = "Character window thread";
            charWindowThread.SetApartmentState(ApartmentState.STA);
            charWindowThread.Start();
            Debug.WriteLine("Char window loaded");
        }
        public static void LoadCooldownWindow()
        {
            var cooldownWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                CooldownWindow = new CooldownWindow();
                CooldownWindow.AllowsTransparency = SettingsManager.CooldownWindowSettings.AllowTransparency;

                CooldownWindow.Show();
                waiting = false;
                Dispatcher.Run();
            }));
            cooldownWindowThread.Name = "Cooldown bar thread";
            cooldownWindowThread.SetApartmentState(ApartmentState.STA);
            cooldownWindowThread.Start();
            Debug.WriteLine("Cd window loaded");


        }
        public static void LoadBossGaugeWindow()
        {

            var bossGaugeThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BossWindow = new BossWindow();

                BossWindow.AllowsTransparency = SettingsManager.BossWindowSettings.AllowTransparency;
                BossWindow.Show();
                waiting = false;

                Dispatcher.Run();
            }));
            bossGaugeThread.Name = "Boss gauge thread";
            bossGaugeThread.SetApartmentState(ApartmentState.STA);
            bossGaugeThread.Start();
            Debug.WriteLine("Boss window loaded");

        }
        public static void LoadBuffBarWindow()
        {
            var buffBarThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BuffWindow = new BuffWindow();
                BuffBarWindowViewModel.Instance.Player = new Data.Player();
                BuffWindow.Show();
                waiting = false;

                Dispatcher.Run();
            }));
            buffBarThread.Name = "Buff bar thread";
            buffBarThread.SetApartmentState(ApartmentState.STA);
            buffBarThread.Start();
            Debug.WriteLine("Buff window loaded");


        }
        public static void LoadGroupWindow()
        {
            var groupWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                GroupWindow = new GroupWindow();
                GroupWindow.Show();
                waiting = false;

                Dispatcher.Run();
            }));
            groupWindowThread.Name = "Group window thread";
            groupWindowThread.SetApartmentState(ApartmentState.STA);
            groupWindowThread.Start();
            Debug.WriteLine("Group window loaded");

        }
        public static void LoadChatWindow()
        {
            var chatWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                ChatWindow = new ChatWindow();
                ChatWindow.AllowsTransparency = SettingsManager.ChatWindowSettings.AllowTransparency;
                ChatWindow.Show();
                waiting = false;

                Dispatcher.Run();
            }));
            chatWindowThread.Name = "Chat thread";
            chatWindowThread.SetApartmentState(ApartmentState.STA);
            chatWindowThread.Start();
            Debug.WriteLine("Chat window loaded");

        }
        public static void LoadClassWindow()
        {      
            var t = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                ClassWindow = new ClassWindow();
                ClassWindow.Closed += (s, ev) => ClassWindow.Dispatcher.InvokeShutdown();
                ClassWindow.Show();
                waiting = false;

                Dispatcher.Run();
            }));
            t.Name = "Class bar thread";
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            Debug.WriteLine("Class window loaded");


        }
        static bool waiting;
        static void LoadWindows()
        {
            waiting = true;
            foreach (var del in WindowLoadingDelegates)
            {
                waiting = true;
                del.DynamicInvoke();
                while (waiting) { }
            }
            Debug.WriteLine("Windows loaded");

        }
        public static void Init()
        {
            LoadWindows();
            ContextMenu = new ContextMenu();
            DefaultIcon = new Icon(Application.GetResourceStream(new Uri("resources/tcc-logo.ico", UriKind.Relative)).Stream);
            ConnectedIcon = new Icon(Application.GetResourceStream(new Uri("resources/tcc-logo-on.ico", UriKind.Relative)).Stream);
            TrayIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = DefaultIcon,
                Visible = true
            };
            TrayIcon.MouseDown += NI_MouseDown;
            TrayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            TrayIcon.Text = String.Format("TCC v{0}.{1}.{2}", v.Major, v.Minor, v.Build);
            var CloseButton = new MenuItem() { Header = "Close" };

            CloseButton.Click += (s, ev) => App.CloseApp();
            ContextMenu.Items.Add(CloseButton);

            _undimTimer = new System.Timers.Timer(5000);
            _undimTimer.Elapsed += _undimTimer_Elapsed;

            FocusManager.FocusTimer = new System.Timers.Timer(1000);
            FocusManager.FocusTimer.Elapsed += FocusManager.CheckForegroundWindow;

            ClickThruChanged += (s, ev) => UpdateClickThru();
        }

        private static void _undimTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SkillsEnded = true;
            _undimTimer.Stop();
        }

        private static void TrayIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Settings == null)
            {
                Settings = new SettingsWindow()
                {
                    Name = "Settings"
                };
            }
            Settings.Opacity = 0;
            Settings.Show();
            Settings.BeginAnimation(Window.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
        }

        public static void Dispose()
        {
            FocusManager.FocusTimer.Stop();
            TrayIcon.Visible = false;


            foreach (Window w in Application.Current.Windows)
            {
                if(w.GetType() == typeof(TccWindow))
                {
                    ((TccWindow)w).CloseWindowSafe();
                }
                else
                {
                    w.Close();
                }
            }
        }

        private static void SetClickThru()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(SettingsWindow)) continue;
                FocusManager.MakeTransparent(new WindowInteropHelper(w).Handle);
            }
        }
        private static void UnsetClickThru()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(SettingsWindow)) continue;
                FocusManager.UndoTransparent(new WindowInteropHelper(w).Handle);
            }

        }
        private static void UpdateClickThru()
        {
            if (ClickThru)
            {
                SetClickThru();
            }
            else
            {
                UnsetClickThru();
            }

        }
        private static void NI_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu.IsOpen = true;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ContextMenu.IsOpen = false;
            }
        }
    }
}
