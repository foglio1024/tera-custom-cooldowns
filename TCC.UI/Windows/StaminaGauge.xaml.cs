using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.Parsing;

namespace TCC
{
    public delegate void StaminaCapEvent();

    /// <summary>
    /// Logica di interazione per StaminaGauge.xaml
    /// </summary>
    public partial class StaminaGauge : Window
    {
        public event StaminaCapEvent Maxed;
        public event StaminaCapEvent UnMaxed;
        //static int MaxStamina = 1;
        //public static bool Visible { get; set; }
        bool maxed;
        bool IsMaxed {
            get
            {
                return maxed;
            }
            set
            {
                if(value != maxed)
                {
                    maxed = value;
                    if (maxed)
                    {
                        Maxed?.Invoke();
                    }
                    else
                    {
                        UnMaxed?.Invoke();
                    }

                }
            }
        }
        private System.Windows.Media.Color color;
        
        public StaminaGauge(System.Windows.Media.Color color)
        {
            InitializeComponent();
            this.color = color;
            Maxed += StaminaGauge_Maxed;
            UnMaxed += StaminaGauge_Unmaxed;
            SessionManager.CurrentPlayer.STUpdated += StaminaGauge_StaminaChanged;
            //PacketRouter.MaxSTUpdated += PacketParser_MaxSTUpdated;

            Left = Properties.Settings.Default.ClassGaugeLeft;
            Top = Properties.Settings.Default.ClassGaugeTop;

            StaminaAmount.EndAngle = 0;

            StaminaAmount.Stroke = new SolidColorBrush(color);
            glow.Color = color;
            baseEll.Fill = new SolidColorBrush(Colors.Transparent);
        }

        //private void PacketParser_MaxSTUpdated(int statValue)
        //{
        //    MaxStamina = statValue;
        //}
        float oldStamina;
        private void StaminaGauge_StaminaChanged(float stamina)
        {
            Dispatcher.Invoke(() => 
            {
                num.Text = stamina.ToString();
                double newAngle = ValueToAngle(Convert.ToInt32(stamina));
                DoubleAnimation anim = new DoubleAnimation(newAngle, TimeSpan.FromMilliseconds(100)) { EasingFunction = new QuadraticEase() };
                StaminaAmount.BeginAnimation(Arc.EndAngleProperty, anim);

                if(stamina == SessionManager.CurrentPlayer.MaxST)
                {
                    IsMaxed = true;
                }
                else
                {
                    IsMaxed = false;
                }

                oldStamina = stamina;
            });
        }



        double ValueToAngle(int val)
        {
            if (359.9 * ((double)val / (double)SessionManager.CurrentPlayer.MaxST) != Double.NaN)
            {
                return 359.9 * ((double)val / (double)SessionManager.CurrentPlayer.MaxST);
            }
            else
            {
                return 0;
            }
        }
        private void StaminaGauge_Maxed()
        {
            Dispatcher.Invoke(() =>
            {
                glow.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)));
                baseEll.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(color, TimeSpan.FromMilliseconds(200)));
            });
        }
        private void StaminaGauge_Unmaxed()
        {
            Dispatcher.Invoke(() =>
            {
                StaminaAmount.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(color, TimeSpan.FromMilliseconds(200)));
                baseEll.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(System.Windows.Media.Color.FromArgb(0xff, 0x20, 0x20, 0x20), TimeSpan.FromMilliseconds(200)));
                glow.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(200)));
            });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            Opacity = 0;
            ContextMenu = new ContextMenu();
            var HideButton = new MenuItem() { Header = "Hide" };
            HideButton.Click += (s, ev) => 
            {
                this.Visibility = Visibility.Collapsed;
            };
            ContextMenu.Items.Add(HideButton);

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Properties.Settings.Default.ClassGaugeLeft = Left;
            //Properties.Settings.Default.ClassGaugeTop= Top;
            //Properties.Settings.Default.Save();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
