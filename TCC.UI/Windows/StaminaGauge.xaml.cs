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
    public delegate void MaxStamina();

    /// <summary>
    /// Logica di interazione per StaminaGauge.xaml
    /// </summary>
    public partial class StaminaGauge : Window
    {
        public event MaxStamina Maxed;
        static int MaxStamina = 1;
        public static bool Visible { get; set; }
        private System.Windows.Media.Color color;
        
        public StaminaGauge(System.Windows.Media.Color color)
        {
            InitializeComponent();
            this.color = color;
            Maxed += StaminaGauge_Maxed;
            PacketRouter.STUpdated += StaminaGauge_StaminaChanged;
            PacketRouter.MaxSTUpdated += PacketParser_MaxSTUpdated;

            Left = Properties.Settings.Default.GaugeWindowLeft;
            Top = Properties.Settings.Default.GaugeWindowTop;

            StaminaAmount.EndAngle = 0;

            StaminaAmount.Stroke = new SolidColorBrush(color);
            glow.Color = color;
            baseEll.Fill = new SolidColorBrush(Colors.Transparent);
        }

        private void PacketParser_MaxSTUpdated(int statValue)
        {
            MaxStamina = statValue;
        }

        private void StaminaGauge_StaminaChanged(int stamina)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                num.Text = stamina.ToString();

                StaminaAmount.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(ValueToAngle(stamina), TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });

                if(stamina == MaxStamina)
                {
                    Maxed?.Invoke();
                }
                else
                {
                    StaminaAmount.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(color, TimeSpan.FromMilliseconds(200)));

                    baseEll.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(System.Windows.Media.Color.FromArgb(0xff, 0x20, 0x20, 0x20), TimeSpan.FromMilliseconds(200)));

                    glow.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(200)));

                }

            }));
        }

        double ValueToAngle(int val)
        {
            if (359.9 * ((double)val / (double)MaxStamina) != Double.NaN)
            {
                return 359.9 * ((double)val / (double)MaxStamina);
            }
            else
            {
                return 0;
            }
        }
        private void StaminaGauge_Maxed()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                glow.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)));

                baseEll.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(color, TimeSpan.FromMilliseconds(200)));
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            if (Properties.Settings.Default.Transparent)
            {
                FocusManager.MakeTransparent(hwnd);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.GaugeWindowLeft = Left;
            Properties.Settings.Default.GaugeWindowTop = Top;
        }

    }
}
