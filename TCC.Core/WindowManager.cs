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
using System.Windows.Threading;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public static class WindowManager
    {
        public static CooldownWindow CooldownWindow;
        public static CharacterWindow CharacterWindow;
        public static BossGageWindow BossGauge;
        public static AbnormalitiesWindow BuffBar;
        public static GroupWindow GroupWindow;
        public static TccWindow ClassWindow;

        public static SettingsWindow Settings;

        public static ContextMenu ContextMenu;

        static System.Windows.Forms.NotifyIcon TrayIcon;

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
                if (SessionManager.Logged && !SessionManager.LoadingScreen && IsFocused)
                {
                    isTccVisible = true;
                    return isTccVisible;
                }
                else
                {
                    isTccVisible = false;
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
                if (isFocused != value)
                {
                    isFocused = value;
                    NotifyVisibilityChanged();
                }
            }
        }

        public static void NotifyVisibilityChanged()
        {
            TccVisibilityChanged?.Invoke(null, new PropertyChangedEventArgs("IsTeraOnTop"));
        }

        public static event PropertyChangedEventHandler ClickThruChanged;
        public static event PropertyChangedEventHandler TccVisibilityChanged;

        public static Visibility StaminaGaugeVisibility;
        public static double StaminaGaugeTop;
        public static double StaminaGaugeLeft;

        public static void Init()
        {

            var charWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                CharacterWindow = new CharacterWindow();
                CharacterWindowManager.Instance.Player = new Data.Player();
                CharacterWindow.Show();
                Dispatcher.Run();
            }));
            charWindowThread.Name = "Character window thread";
            charWindowThread.SetApartmentState(ApartmentState.STA);
            charWindowThread.Start();

            var cooldownWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                CooldownWindow = new CooldownWindow();
                CooldownBarWindowManager.Instance.ShortSkills = new SynchronizedObservableCollection<SkillCooldown>(CooldownWindow.Dispatcher);
                CooldownBarWindowManager.Instance.LongSkills = new SynchronizedObservableCollection<SkillCooldown>(CooldownWindow.Dispatcher);
                CooldownWindow.Show();
                Dispatcher.Run();
            }));
            cooldownWindowThread.Name = "Cooldown bar thread";
            cooldownWindowThread.SetApartmentState(ApartmentState.STA);
            cooldownWindowThread.Start();

            var bossGaugeThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BossGauge = new BossGageWindow();
                BossGageWindowManager.Instance.CurrentNPCs = new SynchronizedObservableCollection<Data.Boss>(BossGauge.Dispatcher);
                BossGauge.Show();
                Dispatcher.Run();
            }));
            bossGaugeThread.Name = "Boss gauge thread";
            bossGaugeThread.SetApartmentState(ApartmentState.STA);
            bossGaugeThread.Start();

            var buffBarThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                BuffBar = new AbnormalitiesWindow();
                BuffBarWindowManager.Instance.Player = new Data.Player();
                BuffBar.Show();
                Dispatcher.Run();
            }));
            buffBarThread.Name = "Buff bar thread";
            buffBarThread.SetApartmentState(ApartmentState.STA);
            buffBarThread.Start();

            var groupWindowThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                GroupWindow = new GroupWindow();
                GroupWindowManager.Instance.Dps = new SynchronizedObservableCollection<Data.User>(GroupWindow.Dispatcher);
                GroupWindowManager.Instance.Healers = new SynchronizedObservableCollection<Data.User>(GroupWindow.Dispatcher);
                GroupWindowManager.Instance.Tanks = new SynchronizedObservableCollection<Data.User>(GroupWindow.Dispatcher);
                GroupWindow.Show();
                Dispatcher.Run();
            }));
            groupWindowThread.Name = "Group window thread";
            groupWindowThread.SetApartmentState(ApartmentState.STA);
            groupWindowThread.Start();

            var t = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                ClassWindow = new WarriorBar();
                WarriorBarManager.Instance.SecondarySkills = new SynchronizedObservableCollection<Data.FixedSkillCooldown>(ClassWindow.Dispatcher);
                WarriorBarManager.Instance.MainSkills = new SynchronizedObservableCollection<Data.FixedSkillCooldown>(ClassWindow.Dispatcher);
                WarriorBarManager.Instance.OtherSkills = new SynchronizedObservableCollection<SkillCooldown>(ClassWindow.Dispatcher);
                WarriorBarManager.Instance.LoadSkills();
                ClassWindow.Closed += (s, ev) => ClassWindow.Dispatcher.InvokeShutdown();
                ClassWindow.Show();
                Dispatcher.Run();
            }))
            {
                Name = "Warrior bar thread"
            };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            Thread.Sleep(500);
            ContextMenu = new ContextMenu();

            TrayIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
                Visible = true
            };
            TrayIcon.MouseDown += NI_MouseDown;
            TrayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            TrayIcon.Text = String.Format("TCC v{0}.{1}.{2}", v.Major, v.Minor, v.Build);
            var CloseButton = new MenuItem() { Header = "Close" };

            CloseButton.Click += (s, ev) => App.CloseApp();
            ContextMenu.Items.Add(CloseButton);


            FocusManager.FocusTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            FocusManager.FocusTimer.Tick += FocusManager.CheckForegroundWindow;

            ClickThruChanged += (s, ev) => UpdateClickThru();
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

        internal static void InitClassBar()
        {
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    break;
                case Class.Lancer:
                    break;
                case Class.Slayer:
                    break;
                case Class.Berserker:
                    break;
                case Class.Sorcerer:
                    break;
                case Class.Archer:
                    break;
                case Class.Priest:
                    break;
                case Class.Elementalist:
                    break;
                case Class.Soulless:
                    break;
                case Class.Engineer:
                    break;
                case Class.Fighter:
                    break;
                case Class.Assassin:
                    break;
                case Class.Glaiver:
                    break;
                case Class.Common:
                    break;
                case Class.None:
                    break;
                default:
                    break;
            }
        }

        public static void ChangeClickThru(bool v)
        {
            if (v)
            {
                ClickThru = true;
                //ClickThruButton.IsChecked = true;
                SetClickThru();
            }
            else
            {
                ClickThru = false;
                //ClickThruButton.IsChecked = false;
                UnsetClickThru();
            }
        }

        public static void Dispose()
        {
            FocusManager.FocusTimer.Stop();
            TrayIcon.Visible = false;


            foreach (Window w in Application.Current.Windows)
            {
                w.Close();
            }
        }
        //public static void InitClassGauge(Class c)
        //{

        //    switch (c)
        //    {
        //        case Class.Engineer:
        //            App.Current.Dispatcher.Invoke(() =>
        //            {
        //                ClassSpecificWindow.Init(Colors.Orange);
        //                ClassSpecificWindow.Enabled = true;
        //            });
        //            break;
        //        case Class.Fighter:
        //            App.Current.Dispatcher.Invoke(() =>
        //            {
        //                ClassSpecificWindow.Init(Colors.OrangeRed);
        //                ClassSpecificWindow.Enabled = true;
        //            });
        //            break;
        //        case Class.Assassin:
        //            App.Current.Dispatcher.Invoke(() =>
        //            {
        //                ClassSpecificWindow.Init(System.Windows.Media.Color.FromArgb(0xff,0xff,0x6a,0xff));
        //                ClassSpecificWindow.Enabled = true;
        //            });
        //            break;
        //        case Class.Glaiver:
        //            App.Current.Dispatcher.Invoke(() =>
        //            {
        //                ClassSpecificWindow.Init(System.Windows.Media.Color.FromRgb(230,240,255));
        //                ClassSpecificWindow.Enabled = true;
        //            });
        //            break;
        //        default:
        //            ClassSpecificWindow.Enabled = false;
        //            return;
        //    }
        //    if (Transparent)
        //    {
        //        App.Current.Dispatcher.Invoke(() =>
        //        {
        //            FocusManager.MakeTransparent(new WindowInteropHelper(ClassSpecificWindow).Handle);
        //        });
        //    }
        //}
        public static void ShowWindow(Window w)
        {
            if (w != null)
            {
                w.Dispatcher.BeginInvoke(new Action(() =>
                {
                    w.BeginAnimation(Window.OpacityProperty, OpacityAnimation(1));
                }));
            }

        }
        public static void HideWindow(Window w)
        {
            if (w != null)
            {
                w.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var a = OpacityAnimation(0);
                    w.BeginAnimation(Window.OpacityProperty, a);
                }));
            }
        }

        static bool isForeground = false;
        private static void FocusManager_ForegroundWindowChanged(bool visible)
        {
            if (visible && SessionManager.Logged && !SessionManager.LoadingScreen)
            {
                foreach (Window w in Application.Current.Windows)
                {
                    ShowWindow(w);
                    if (!isForeground)
                    {
                        w.Topmost = false;
                        w.Topmost = true;
                    }
                }
                isForeground = true;

            }
            else
            {
                foreach (Window w in Application.Current.Windows)
                {
                    if (w.Name == "Settings") continue;
                    HideWindow(w);
                }
                isForeground = false;
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
        private static void ToggleClickThru()
        {
            if (ClickThru)
            {
                UnsetClickThru();
                //ClickThruButton.IsChecked = false;
                ClickThru = false;
            }
            else
            {
                SetClickThru();
                //ClickThruButton.IsChecked = true;
                ClickThru = true;
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
        private static DoubleAnimation OpacityAnimation(double to)
        {
            return new DoubleAnimation(to, TimeSpan.FromMilliseconds(300)) { EasingFunction = new QuadraticEase() };
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
