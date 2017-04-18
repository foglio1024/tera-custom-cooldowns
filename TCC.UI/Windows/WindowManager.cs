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
        //public static StaminaGauge ClassSpecificWindow;
        public static CharacterWindow CharacterWindow;
        public static BossGageWindow BossGauge;
        public static AbnormalitiesWindow BuffBar;
        public static ContextMenu ContextMenu;
        static MenuItem ClickThruButton;
        static MenuItem CharacterWindowVisibilityButton;
        static MenuItem CooldownWindowVisibilityButton;
        //static MenuItem ClassSpecificWindowVisibilityButton;
        static MenuItem BossGaugeWindowVisibilityButton;
        static MenuItem BuffBarWindowVisibilityButton;
        static System.Windows.Forms.NotifyIcon TrayIcon;

        public static bool Transparent;
        static bool IsClassWindowVisible;

        public static Visibility StaminaGaugeVisibility;
        public static double StaminaGaugeTop;
        public static double StaminaGaugeLeft;

        public static void Init()
        {
            CooldownWindow = new CooldownWindow();
            //ClassSpecificWindow = new StaminaGauge();
            CharacterWindow = new CharacterWindow();
            BossGauge = new BossGageWindow();
            BuffBar = new AbnormalitiesWindow();

            ContextMenu = new ContextMenu();
            
            TrayIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
                Visible = true
            };
            TrayIcon.MouseDown += NI_MouseDown;
            var ForceShowButton = new MenuItem() { Header = "Force visibility on" };
            ClickThruButton = new MenuItem() { Header = "Click through"};
            var CloseButton = new MenuItem() { Header = "Close" };

            CharacterWindowVisibilityButton = new MenuItem() { Header = "Unhide character window" };
            CharacterWindowVisibilityButton.Click += (s, ev) =>
            {
                CharacterWindow.Visibility = Visibility.Visible;
            };
            CooldownWindowVisibilityButton  = new MenuItem() { Header = "Unhide cooldowns bar" };
            CooldownWindowVisibilityButton.Click += (s, ev) =>
            {
                CooldownWindow.Visibility = Visibility.Visible;
            };
            
            BossGaugeWindowVisibilityButton = new MenuItem() { Header = "Unhide boss bar" };
            BossGaugeWindowVisibilityButton.Click += (s, ev) =>
            {
                BossGauge.Visibility = Visibility.Visible;
            };
            BuffBarWindowVisibilityButton = new MenuItem() { Header = "Unhide buffs bar" };
            BuffBarWindowVisibilityButton.Click += (s, ev) =>
            {
                BuffBar.Visibility = Visibility.Visible;
            };


            CloseButton.Click += (s, ev) => App.CloseApp();
            ClickThruButton.Click += (s, ev) => ToggleClickThru();
            ForceShowButton.Click += (s, ev) => ForceShow();

            ContextMenu.Items.Add(CooldownWindowVisibilityButton);
            ContextMenu.Items.Add(BuffBarWindowVisibilityButton);
            ContextMenu.Items.Add(BossGaugeWindowVisibilityButton);
            //ContextMenu.Items.Add(ClassSpecificWindowVisibilityButton);
            ContextMenu.Items.Add(CharacterWindowVisibilityButton);
            ContextMenu.Items.Add(new Separator());
            ContextMenu.Items.Add(ForceShowButton);
            ContextMenu.Items.Add(ClickThruButton);
            ContextMenu.Items.Add(CloseButton);


            FocusManager.FocusTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            FocusManager.FocusTimer.Tick += FocusManager.CheckForegroundWindow;
            SessionManager.CurrentPlayer.InCombat += SessionManager_InCombat;
            SessionManager.CurrentPlayer.OutOfCombat += SessionManager_OutOfCombat;

            FocusManager.ForegroundWindowChanged += FocusManager_ForegroundWindowChanged;
        }

        private static void ForceShow()
        {
            CooldownWindow.Show();
            CooldownWindow.Topmost = true;
            CharacterWindow.Show();
            CharacterWindow.Topmost = true;
            BossGauge.Show();
            BossGauge.Topmost = true;
            BuffBar.Show();
            BuffBar.Topmost = true;
        }

        public static void ChangeClickThru(bool v)
        {
            if (v)
            {
                Transparent = true;
                ClickThruButton.IsChecked = true;
                SetClickThru();
            }
            else
            {
                Transparent = false;
                ClickThruButton.IsChecked = false;
                UnsetClickThru();
            }
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
                    //w.Show();
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
                    //a.Completed += (s, ev) => w.Hide();
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
                    //HideWindow(ClassSpecificWindow);
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
                ShowWindow(BossGauge);
                CooldownWindow.Topmost = true;
                CharacterWindow.Topmost = true;
                BuffBar.Topmost = true;
                BossGauge.Topmost = true;
                //ShowWindow(ClassSpecificWindow);
                //TODO: add setting for gauge visibility
                //switch (SessionManager.CurrentPlayer.Class)
                //{
                //    //case Class.Warrior:
                //    //    if (IsClassWindowVisible)
                //    //    {
                //    //        ShowWindow(ClassSpecificWindow);
                //    //    }
                //    //    break;
                //    case Class.Engineer:
                //        if (IsClassWindowVisible)
                //        {
                //            ShowWindow(ClassSpecificWindow);
                //        }
                //        break;
                //    case Class.Fighter:
                //        if (IsClassWindowVisible)
                //        {
                //            ShowWindow(ClassSpecificWindow);
                //        }
                //        break;
                //    case Class.Assassin:
                //        if (IsClassWindowVisible)
                //        {
                //            ShowWindow(ClassSpecificWindow);
                //        }
                //        break;
                //    case Class.Glaiver:
                //        if (IsClassWindowVisible)
                //        {
                //            ShowWindow(ClassSpecificWindow);
                //        }
                //        break;
                //    default:
                //        break;
                //}
            }
            else
            {
                HideWindow(CooldownWindow);
                HideWindow(CharacterWindow);
                //HideWindow(ClassSpecificWindow);
                HideWindow(BossGauge);
                HideWindow(BuffBar);
            }

            //if(visible && SessionManager.Logged)
            //{
            //    BossGauge.Show();
            //}
            //else
            //{
            //    BossGauge.Hide();
            //}
        }
        private static void SetClickThru()
        {
            FocusManager.MakeTransparent(new WindowInteropHelper(BossGauge).Handle);
            FocusManager.MakeTransparent(new WindowInteropHelper(BuffBar).Handle);
            FocusManager.MakeTransparent(new WindowInteropHelper(CooldownWindow).Handle);
            FocusManager.MakeTransparent(new WindowInteropHelper(CharacterWindow).Handle);
            //FocusManager.MakeTransparent(new WindowInteropHelper(ClassSpecificWindow).Handle);

        }
        private static void UnsetClickThru()
        {
            FocusManager.UndoTransparent(new WindowInteropHelper(BossGauge).Handle);
            FocusManager.UndoTransparent(new WindowInteropHelper(BuffBar).Handle);
            FocusManager.UndoTransparent(new WindowInteropHelper(CharacterWindow).Handle);
            FocusManager.UndoTransparent(new WindowInteropHelper(CooldownWindow).Handle);
            //FocusManager.UndoTransparent(new WindowInteropHelper(ClassSpecificWindow).Handle);

        }
        private static void ToggleClickThru()
        {
            if (Transparent)
            {
                UnsetClickThru();
                ClickThruButton.IsChecked = false;
                Transparent = false;
            }
            else
            {
                SetClickThru();
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
