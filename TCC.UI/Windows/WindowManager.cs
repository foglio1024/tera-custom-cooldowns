using DamageMeter.Sniffing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Windows;

namespace TCC
{
    public static class WindowManager
    {
        public static CooldownWindow CooldownWindow;
        public static Window ClassSpecificWindow;
        public static CharacterWindow CharacterWindow;
        public static BossGageWindow BossGauge;
        public static AbnormalitiesWindow BuffBar;
        static ContextMenu ContextMenu;
        static MenuItem ClickThruButton;
        static System.Windows.Forms.NotifyIcon TrayIcon;

        public static bool Transparent;
        static bool IsClassWindowVisible;

        public static void Init()
        {
            CooldownWindow = new CooldownWindow();
            ClassSpecificWindow = new Window();
            CharacterWindow = new CharacterWindow();
            BossGauge = new BossGageWindow();
            BuffBar = new AbnormalitiesWindow();

            /*TODO: move to each window*/
            CooldownWindow.Opacity = 0;
            ClassSpecificWindow.Opacity = 0;
            CharacterWindow.Opacity = 0;

            //CooldownWindow.Top = Properties.Settings.Default.CooldownBarTop;
            //CooldownWindow.Left = Properties.Settings.Default.CooldownBarLeft;
            //ClassSpecificWindow.Top = Properties.Settings.Default.ClassGaugeTop;
            //ClassSpecificWindow.Left = Properties.Settings.Default.ClassGaugeLeft;
            //CharacterWindow.Top = Properties.Settings.Default.CharacterWindowTop;
            //CharacterWindow.Left = Properties.Settings.Default.CharacterWindowLeft;
            //BossGauge.Top = Properties.Settings.Default.BossGaugeWindowTop;
            //BossGauge.Left = Properties.Settings.Default.BossGaugeWindowLeft;
            //BuffBar.Top = Properties.Settings.Default.BuffBarTop;
            //BuffBar.Left = Properties.Settings.Default.BuffBarLeft;

            ContextMenu = new ContextMenu();
            
            TrayIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
                Visible = true
            };
            TrayIcon.MouseDown += NI_MouseDown;

            ClickThruButton = new MenuItem() { Header = "Click through"};
            var CloseButton = new MenuItem() { Header = "Close" };

            CloseButton.Click += (s, ev) => App.CloseApp();
            ClickThruButton.Click += (s, ev) => SetTransparentWindows();

            ContextMenu.Items.Add(ClickThruButton);
            ContextMenu.Items.Add(CloseButton);


            FocusManager.FocusTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            FocusManager.FocusTimer.Tick += FocusManager.CheckForegroundWindow;
            SessionManager.CurrentPlayer.InCombat += SessionManager_InCombat;
            SessionManager.CurrentPlayer.OutOfCombat += SessionManager_OutOfCombat;

            FocusManager.ForegroundWindowChanged += FocusManager_ForegroundWindowChanged;
        }
        public static void SetTransparent(bool v)
        {
            if (v)
            {
                Transparent = true;
                ClickThruButton.IsChecked = true;
            }
            else
            {
                Transparent = false;
                ClickThruButton.IsChecked = false;
            }

            SetTransparentWindows();
        }

        public static void Dispose()
        {
            FocusManager.FocusTimer.Stop();
            TrayIcon.Visible = false;

            //Properties.Settings.Default.CooldownBarLeft = CooldownWindow.Left;
            //Properties.Settings.Default.CooldownBarTop= CooldownWindow.Top;
            //Properties.Settings.Default.CharacterWindowLeft = CharacterWindow.Left;
            //Properties.Settings.Default.CharacterWindowTop = CharacterWindow.Top;
            //Properties.Settings.Default.BuffBarTop = BuffBar.Top;
            //Properties.Settings.Default.BuffBarLeft = BuffBar.Left;
            //Properties.Settings.Default.BossGaugeWindowTop = BossGauge.Top;
            //Properties.Settings.Default.BossGaugeWindowLeft = BossGauge.Left;

            //if(ClassSpecificWindow != null)
            //{
            //    ClassSpecificWindow.Close();
            //    Properties.Settings.Default.ClassGaugeLeft = ClassSpecificWindow.Left;
            //    Properties.Settings.Default.ClassGaugeTop = ClassSpecificWindow.Top;
            //}
            //Properties.Settings.Default.Transparent = Transparent;

            //Properties.Settings.Default.Save();
            App.SaveSettings();
            CooldownWindow.Close();
            CharacterWindow.Close();
        }
        public static void InitClassGauge(Class c)
        {
            if (ClassSpecificWindow != null)
            {
                ClassSpecificWindow.Dispatcher.BeginInvoke(new Action(() => ClassSpecificWindow.Close()));
            }

            switch (c)
            {
                case Class.Warrior:
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClassSpecificWindow = new EdgeGaugeWindow();
                    });
                    break;
                case Class.Engineer:
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClassSpecificWindow = new StaminaGauge(Colors.Orange);
                    });
                    break;
                case Class.Fighter:
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClassSpecificWindow = new StaminaGauge(Colors.OrangeRed);
                    });
                    break;
                case Class.Assassin:
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClassSpecificWindow = new StaminaGauge(System.Windows.Media.Color.FromArgb(0xff,0xff,0x6a,0xff));
                    });
                    break;
                case Class.Glaiver:
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClassSpecificWindow = new StaminaGauge(System.Windows.Media.Color.FromRgb(230,240,255));
                    });
                    break;
                default:
                    return;
            }
            if (Transparent)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    FocusManager.MakeTransparent(new WindowInteropHelper(ClassSpecificWindow).Handle);
                });
            }
        }
        public static void ShowWindow(Window w)
        {
            if (w != null)
            {
                w.Dispatcher.BeginInvoke(new Action(() =>
                {
                    w.Show();
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
                    a.Completed += (s, ev) => w.Hide();
                    w.BeginAnimation(Window.OpacityProperty, a);
                }));
            }
        }
        private static void SessionManager_InCombat()
        {
            IsClassWindowVisible = true;
        }
        private static void SessionManager_OutOfCombat()
        {
            var t = new Timer(5000);
            t.Elapsed += (s, o) =>
            {
                if (!SessionManager.CurrentPlayer.IsInCombat)
                {
                    IsClassWindowVisible = false;
                    HideWindow(ClassSpecificWindow);
                }
                t.Stop();
            };
            t.Enabled=true;
        }
        private static void FocusManager_ForegroundWindowChanged(bool visible)
        {
            if (visible && SessionManager.Logged)
            {
                ShowWindow(CooldownWindow);
                ShowWindow(CharacterWindow);
                ShowWindow(BuffBar);

                //TODO: add setting for gauge visibility
                switch (SessionManager.CurrentPlayer.Class)
                {
                    case Class.Warrior:
                        if (IsClassWindowVisible)
                        {
                            ShowWindow(ClassSpecificWindow);
                        }
                        break;
                    case Class.Engineer:
                        if (IsClassWindowVisible)
                        {
                            ShowWindow(ClassSpecificWindow);
                        }
                        break;
                    case Class.Fighter:
                        if (IsClassWindowVisible)
                        {
                            ShowWindow(ClassSpecificWindow);
                        }
                        break;
                    case Class.Assassin:
                        if (IsClassWindowVisible)
                        {
                            ShowWindow(ClassSpecificWindow);
                        }
                        break;
                    case Class.Glaiver:
                        if (IsClassWindowVisible)
                        {
                            ShowWindow(ClassSpecificWindow);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                HideWindow(CooldownWindow);
                HideWindow(CharacterWindow);
                HideWindow(ClassSpecificWindow);
            }

            if(visible && SessionManager.Logged)
            {
                BossGauge.Show();
            }
            else
            {
                BossGauge.Hide();
            }
        }
        private static void SetTransparentWindows()
        {
            if (Transparent)
            {
                FocusManager.UndoTransparent(new WindowInteropHelper(BossGauge).Handle);
                FocusManager.UndoTransparent(new WindowInteropHelper(BuffBar).Handle);
                FocusManager.UndoTransparent(new WindowInteropHelper(CharacterWindow).Handle);
                FocusManager.UndoTransparent(new WindowInteropHelper(CooldownWindow).Handle);
                if (ClassSpecificWindow != null)
                {
                    FocusManager.UndoTransparent(new WindowInteropHelper(ClassSpecificWindow).Handle);
                }
                ClickThruButton.IsChecked = false;
                Transparent = false;
            }
            else
            {
                FocusManager.MakeTransparent(new WindowInteropHelper(BossGauge).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(BuffBar).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(CooldownWindow).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(CharacterWindow).Handle);
                if(ClassSpecificWindow != null)
                {
                    FocusManager.MakeTransparent(new WindowInteropHelper(ClassSpecificWindow).Handle);
                }
                ClickThruButton.IsChecked = true;
                Transparent = true;
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
            else if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ContextMenu.IsOpen = false;
            }
        }
    }
}
