using DamageMeter.Sniffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TCC
{
    public static class WindowManager
    {
        public static CooldownsBarWindow CDBar;
        public static EdgeWindow EdgeGauge;
        public static HPWindow HPBars;

        static ContextMenu CM;
        static MenuItem ClickThruButton;
        static System.Windows.Forms.NotifyIcon NI;

        static bool Transparent;


        public static void Init()
        {
            CDBar = new CooldownsBarWindow();
            EdgeGauge = new EdgeWindow();
            HPBars = new HPWindow();

            CM = new ContextMenu();

            NI = new System.Windows.Forms.NotifyIcon()
            {
                Icon = Properties.Resources.tcc,
                Visible = true
            };
            NI.MouseDown += NI_MouseDown;

            ClickThruButton = new MenuItem() { Header = "Click through"};
            var CloseButton = new MenuItem() { Header = "Close" };

            CloseButton.Click += (s, ev) => App.CloseApp();
            ClickThruButton.Click += (s, ev) => SetTransparentWindows();

            CM.Items.Add(CloseButton);
            CM.Items.Add(ClickThruButton);

            Transparent = Properties.Settings.Default.Transparent;

            if (Transparent)
            {
                FocusManager.MakeTransparent(new WindowInteropHelper(CDBar).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(EdgeGauge).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(HPBars).Handle);

                ClickThruButton.IsChecked = true;
            }

            FocusManager.FocusTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            FocusManager.FocusTimer.Tick += FocusManager.CheckForegroundWindow;

            FocusManager.ForegroundWindowChanged += FocusManager_ForegroundWindowChanged;
        }

        private static void FocusManager_ForegroundWindowChanged(bool visible)
        {
            if (visible && PacketParser.Logged)
            {
                CDBar.Show();
                HPBars.Show();
                if (PacketParser.CurrentClass == Class.Warrior)
                {
                    EdgeGauge.Show();
                }
            }
            else
            {
                CDBar.Hide();
                HPBars.Hide();
                EdgeGauge.Hide();
            }

        }

        public static void Dispose()
        {
            FocusManager.FocusTimer.Stop();
            NI.Visible = false;

            Properties.Settings.Default.CooldownBarLeft = CDBar.Left;
            Properties.Settings.Default.CooldownBarTop= CDBar.Top;
            Properties.Settings.Default.EdgeWindowLeft = EdgeGauge.Left;
            Properties.Settings.Default.EdgeWindowTop = EdgeGauge.Top;
            Properties.Settings.Default.HPBarLeft = HPBars.Left;
            Properties.Settings.Default.HPBarTop = HPBars.Top;

            Properties.Settings.Default.Transparent = Transparent;

            Properties.Settings.Default.Save();


            CDBar.Close();
            EdgeGauge.Close();
            HPBars.Close();
        }

        internal static void ShowHPBars()
        {
            HPBars.Dispatcher.Invoke(() => {
                HPBars.Show();
            });
        }
        public static void SetClass(Class c)
        {
            HPBars.CurrentClass = c;
        }

        static void SetTransparentWindows()
        {
            if (Transparent)
            {
                FocusManager.UndoTransparent(new WindowInteropHelper(CDBar).Handle);
                FocusManager.UndoTransparent(new WindowInteropHelper(EdgeGauge).Handle);
                ClickThruButton.IsChecked = false;
                Transparent = false;
            }
            else
            {
                FocusManager.MakeTransparent(new WindowInteropHelper(CDBar).Handle);
                FocusManager.MakeTransparent(new WindowInteropHelper(EdgeGauge).Handle);
                ClickThruButton.IsChecked = true;
                Transparent = true;
            }

        }

        public static void ShowEdgeGauge()
        {
            EdgeGauge.Dispatcher.Invoke(() =>
            {

                EdgeGauge.Show();
            });

        }
        public static void HideEdgeGauge()
        {
            EdgeGauge.Dispatcher.Invoke(() =>
            {

                EdgeGauge.Hide();
            });
        }
        public static void DimEdgeGauge()
        {
            EdgeGauge.Dispatcher.Invoke(() =>
            {
                var a = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = new QuadraticEase()
                };
                EdgeGauge.BeginAnimation(EdgeWindow.OpacityProperty, a);
                Console.WriteLine("Edge dim");
            });

        }
        public static void UndimEdgeGauge()
        {
            EdgeGauge.Dispatcher.Invoke(() =>
            {
                var a = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = new QuadraticEase()
                };
                EdgeGauge.BeginAnimation(EdgeWindow.OpacityProperty, a);
                Console.WriteLine("Edge undim");

            });

        }

        private static void NI_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CM.IsOpen = true;
            }
        }
    }
}
