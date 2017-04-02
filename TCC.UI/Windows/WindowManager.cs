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
        public static Window ClassSpecificGauge;
        public static CharacterWindow CharacterWindow;
        public static BossGageWindow BossGauge;
        public static AbnormalitiesWindow BuffBar;
        static ContextMenu CM;
        static MenuItem ClickThruButton;
        static System.Windows.Forms.NotifyIcon NI;

        static bool Transparent;


        public static void Init()
        {
            CooldownWindow = new CooldownWindow();
            ClassSpecificGauge = new Window();
            CharacterWindow = new CharacterWindow();
            BossGauge = new BossGageWindow();
            BuffBar = new AbnormalitiesWindow();
            /*TODO: move to each window*/
            CooldownWindow.Opacity = 0;
            ClassSpecificGauge.Opacity = 0;
            CharacterWindow.Opacity = 0;

            CM = new ContextMenu();
            
            NI = new System.Windows.Forms.NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
                Visible = true
            };
            NI.MouseDown += NI_MouseDown;

            ClickThruButton = new MenuItem() { Header = "Click through"};
            var CloseButton = new MenuItem() { Header = "Close" };

            CloseButton.Click += (s, ev) => App.CloseApp();
            ClickThruButton.Click += (s, ev) => SetTransparentWindows();

            CM.Items.Add(ClickThruButton);
            CM.Items.Add(CloseButton);

            Transparent = Properties.Settings.Default.Transparent;

            if (Transparent)
            {
                ClickThruButton.IsChecked = true;
            }

            FocusManager.FocusTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            FocusManager.FocusTimer.Tick += FocusManager.CheckForegroundWindow;
            SessionManager.OutOfCombat += SessionManager_OutOfCombat;
            SessionManager.InCombat += SessionManager_InCombat;

            FocusManager.ForegroundWindowChanged += FocusManager_ForegroundWindowChanged;
        }

        private static void SessionManager_InCombat()
        {
            StaminaGauge.Visible = true;
            EdgeGaugeWindow.Visible = true;
        }

        private static void SessionManager_OutOfCombat()
        {
            var t = new Timer(5000);
            t.Elapsed += (s, o) =>
            {
                if (!SessionManager.Combat)
                {
                    StaminaGauge.Visible = false;
                    EdgeGaugeWindow.Visible = false;
                    HideWindow(ClassSpecificGauge);
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
                switch (SessionManager.CurrentClass)
                {
                    case Class.Warrior:
                        if (EdgeGaugeWindow.Visible)
                        {
                            ShowWindow(ClassSpecificGauge);
                        }
                        break;
                    case Class.Engineer:
                        if (StaminaGauge.Visible)
                        {
                            ShowWindow(ClassSpecificGauge);
                        }
                        break;
                    case Class.Fighter:
                        if (StaminaGauge.Visible)
                        {
                            ShowWindow(ClassSpecificGauge);
                        }
                        break;
                    case Class.Assassin:
                        if (StaminaGauge.Visible)
                        {
                            ShowWindow(ClassSpecificGauge);
                        }
                        break;
                    case Class.Valkyrie:
                        if (StaminaGauge.Visible)
                        {
                            ShowWindow(ClassSpecificGauge);
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
                HideWindow(ClassSpecificGauge);
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

        public static void Dispose()
        {
            FocusManager.FocusTimer.Stop();
            NI.Visible = false;

            Properties.Settings.Default.CooldownBarLeft = CooldownWindow.Left;
            Properties.Settings.Default.CooldownBarTop= CooldownWindow.Top;
            Properties.Settings.Default.CharacterWindowLeft = CharacterWindow.Left;
            Properties.Settings.Default.CharacterWindowTop = CharacterWindow.Top;

            if(ClassSpecificGauge != null)
            {
                ClassSpecificGauge.Close();
                Properties.Settings.Default.GaugeWindowLeft = ClassSpecificGauge.Left;
                Properties.Settings.Default.GaugeWindowTop = ClassSpecificGauge.Top;
            }
            Properties.Settings.Default.Transparent = Transparent;

            Properties.Settings.Default.Save();


            CooldownWindow.Close();
            CharacterWindow.Close();
        }

        public static void InitClassGauge(Class c)
        {
            if (ClassSpecificGauge != null)
            {
                ClassSpecificGauge.Dispatcher.BeginInvoke(new Action(() => ClassSpecificGauge.Close()));
            }

            switch (c)
            {
                case Class.Warrior:
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ClassSpecificGauge = new EdgeGaugeWindow();
                    }));
                    break;
                case Class.Engineer:
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ClassSpecificGauge = new StaminaGauge(Colors.Orange);
                    }));
                    break;
                case Class.Fighter:
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ClassSpecificGauge = new StaminaGauge(Colors.OrangeRed);
                    }));
                    break;
                case Class.Assassin:
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ClassSpecificGauge = new StaminaGauge(System.Windows.Media.Color.FromArgb(0xff,0xff,0x6a,0xff));
                    }));
                    break;
                case Class.Valkyrie:
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ClassSpecificGauge = new StaminaGauge(Colors.White);
                    }));
                    break;
                default:
                    return;
            }
        }

        public static void SetCharInfo()
        {
            CharacterWindow.CurrentClass = SessionManager.CurrentClass;
            CharacterWindow.CurrentName = SessionManager.CurrentCharName;
            CharacterWindow.CurrentLaurel = SessionManager.CurrentLaurel;
            CharacterWindow.CurrentLevel = SessionManager.CurrentLevel;
        }

        static void SetTransparentWindows()
        {
            if (Transparent)
            {
                FocusManager.UndoTransparent(new WindowInteropHelper(CooldownWindow).Handle);
                FocusManager.UndoTransparent(new WindowInteropHelper(ClassSpecificGauge).Handle);
                FocusManager.UndoTransparent(new WindowInteropHelper(CharacterWindow).Handle);
                ClickThruButton.IsChecked = false;
                Transparent = false;
            }
            else
            {
                FocusManager.MakeTransparent(new WindowInteropHelper(CooldownWindow).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(ClassSpecificGauge).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(CharacterWindow).Handle);
                ClickThruButton.IsChecked = true;
                Transparent = true;
            }

        }

        public static void ShowWindow(Window w)
        {
            if (w != null)
            {
                w.Dispatcher.BeginInvoke(new Action(() =>
                {
                    w.Show();
                    w.BeginAnimation(EdgeGaugeWindow.OpacityProperty, OpacityAnimation(1));
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
                    w.BeginAnimation(EdgeGaugeWindow.OpacityProperty, a);
                }));
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
                CM.IsOpen = true;
            }
            else if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                CM.IsOpen = false;
            }
        }
    }
}
