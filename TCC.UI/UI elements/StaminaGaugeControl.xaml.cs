using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC
{
    public delegate void StaminaCapEvent();

    /// <summary>
    /// Logica di interazione per StaminaGaugeControl.xaml
    /// </summary>
    public partial class StaminaGaugeControl : UserControl
    {

        public event StaminaCapEvent Maxed;
        public event StaminaCapEvent UnMaxed;

        bool maxed;
        bool IsMaxed
        {
            get
            {
                return maxed;
            }
            set
            {
                if (value != maxed)
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

        public StaminaGaugeControl()
        {
            InitializeComponent();
        }

        public StaminaGaugeControl(System.Windows.Media.Color color)
        {
            InitializeComponent();
            this.color = color;
            Maxed += StaminaGauge_Maxed;
            UnMaxed += StaminaGauge_Unmaxed;
            SessionManager.CurrentPlayer.STUpdated += StaminaGauge_StaminaChanged;

            StaminaAmount.EndAngle = 0;

            StaminaAmount.Stroke = new SolidColorBrush(color);
            glow.Color = color;
            baseEll.Fill = new SolidColorBrush(Colors.Transparent);

        }

        float oldStamina;
        private void StaminaGauge_StaminaChanged(float stamina)
        {
            Dispatcher.Invoke(() =>
            {
                num.Text = stamina.ToString();
                double newAngle = ValueToAngle(Convert.ToInt32(stamina));
                DoubleAnimation anim = new DoubleAnimation(newAngle, TimeSpan.FromMilliseconds(100)) { EasingFunction = new QuadraticEase() };
                StaminaAmount.BeginAnimation(Arc.EndAngleProperty, anim);

                if (stamina == SessionManager.CurrentPlayer.MaxST)
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
        double ValueToAngle(int val)
        {
            if (SessionManager.CurrentPlayer.MaxST != 0)
            {
                return 359.9 * ((double)val / (double)SessionManager.CurrentPlayer.MaxST);
            }
            else
            {
                return 0;
            }
        }


    }
}
