using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using TCC.Data;

namespace TCC.UI_elements
{
    /// <summary>
    /// Logica di interazione per DragonSmallGauge.xaml
    /// </summary>
    public partial class DragonSmallGauge : UserControl
    {
        int AnimationTime = 350;
        float LastEnragePercent = 100;
        int CurrentEnrageTime = 50;
        Timer NumberTimer = new Timer(1000);

        public DragonSmallGauge()
        {
            InitializeComponent();
            //Boss.EnragedChanged += BossGage_EnragedUpdated;
            //Boss.BossHPChanged += BossGage_HPUpdated;

            dragonHPpercTB.DataContext = this;
            dragonNameTB.DataContext = this;
            dragonCol.DataContext = this;
            
            SlideAnimation.EasingFunction = new QuadraticEase();
            ColorChangeAnimation.EasingFunction = new QuadraticEase();
            DoubleAnimation.EasingFunction = new QuadraticEase();
            SlideAnimation.Duration = TimeSpan.FromMilliseconds(250);
            ColorChangeAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
            DoubleAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
            SetEnragePercTB(86);

        }


        bool enraged;
        public bool Enraged
        {
            get { return enraged; }
            set
            {
                if (value != Enraged)
                {
                    if (!value)
                    {
                        LastEnragePercent = CurrentPercentage;
                    }
                    enraged = value;
                }
            }
        }
        float CurrentPercentage
        {
            get
            {
                if(MaxHP != 0)
                {
                    return (CurrentHP / MaxHP) * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string DragonName
        {
            get { return (string)GetValue(DragonNameProperty); }
            set { SetValue(DragonNameProperty, value); }
        }
        public static readonly DependencyProperty DragonNameProperty = DependencyProperty.Register("DragonName", typeof(string), typeof(DragonSmallGauge));

        public ulong EntityId
        {
            get { return (ulong)GetValue(EntityIdProperty); }
            set { SetValue(EntityIdProperty, value); }
        }
        public static readonly DependencyProperty EntityIdProperty = DependencyProperty.Register("EntityId", typeof(ulong), typeof(DragonSmallGauge));

        public float MaxHP
        {
            get { return (float)GetValue(MaxHPProperty); }
            set { SetValue(MaxHPProperty, value); }
        }
        public static readonly DependencyProperty MaxHPProperty = DependencyProperty.Register("MaxHP", typeof(float), typeof(DragonSmallGauge));

        public float CurrentHP
        {
            get { return (float)GetValue(CurrentHPProperty); }
            set { SetValue(CurrentHPProperty, value); }
        }
        public static readonly DependencyProperty CurrentHPProperty = DependencyProperty.Register("CurrentHP", typeof(float), typeof(DragonSmallGauge));

        public SolidColorBrush DragonColor
        {
            get { return (SolidColorBrush)GetValue(DragonColorProperty); }
            set { SetValue(DragonColorProperty, value); }
        }
        public static readonly DependencyProperty DragonColorProperty =DependencyProperty.Register("DragonColor", typeof(SolidColorBrush), typeof(DragonSmallGauge));
        
        void GlowOn()
        {
            Dispatcher.Invoke(() =>
            {
                //DoubleAnimation.To = 1;
                //HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, DoubleAnimation);

                ColorChangeAnimation.To = Colors.Red;
                HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorChangeAnimation);
            });
        }
        void GlowOff()
        {
            Dispatcher.Invoke(() =>
            {
                //DoubleAnimation.To = 0;
                //HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, DoubleAnimation);

                ColorChangeAnimation.To = Color.FromArgb(0xff, 0x4a, 0x82, 0xbd);
                HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorChangeAnimation);
            });
        }
        void SetEnragePercTB(double v)
        {
            Dispatcher.Invoke(() =>
            {
                if (v < 0) v = 0;
                NextEnrageTB.Text = String.Format("{0:0.#}", v);
            });
        }
        void SlideNextEnrage(double val)
        {
            Dispatcher.Invoke(() =>
            {
                if (val < 0)
                {
                    SlideAnimation.To = 0;
                }
                else
                {
                    SlideAnimation.To = BaseRect.ActualWidth * (val / 100);
                }

                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, SlideAnimation);
            });
        }
        private void BossGage_HPUpdated(ulong id, float hp)
        {

            Dispatcher.Invoke(() =>
            {
                if (id == EntityId)
                {
                    CurrentHP = hp;
                    if (CurrentHP > MaxHP)
                    {
                        MaxHP = CurrentHP;
                    }
                    DoubleAnimation.To = ValueToLength(CurrentHP, MaxHP);
                    HPrect.BeginAnimation(WidthProperty, DoubleAnimation);

                    dragonHPpercTB.Text = String.Format("{0:0.0}%", CurrentPercentage);

                    if (Enraged)
                    {
                        SlideNextEnrage(CurrentPercentage);
                        //SetEnragePercTB(CurrentPercentage);
                    }
                    //Console.WriteLine("{0} HP updated.", DragonName);
                }
            });
        }
        private void BossGage_EnragedUpdated(ulong id, bool enraged)
        {
            Dispatcher.Invoke(() =>
            {
                if (id == EntityId)
                {
                    //Console.WriteLine("{0} enraged updated.", DragonName);

                    Enraged = enraged;
                    if (enraged)
                    {
                        Enraged = enraged;
                        NextEnrageTB.Text = CurrentEnrageTime.ToString();

                        SlideNextEnrage(CurrentPercentage);
                        //SetEnragePercTB(CurrentPercentage);

                        GlowOn();
                        NumberTimer = new Timer(1000);
                        NumberTimer.Elapsed += (s, ev) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                NextEnrageTB.Text = CurrentEnrageTime.ToString() + "s";
                                CurrentEnrageTime--;
                            }));
                        };
                        NumberTimer.Enabled = true;

                    }
                    else
                    {
                        if (NumberTimer != null)
                        {
                            NumberTimer.Stop();
                        }

                        GlowOff();

                        SlideNextEnrage(CurrentPercentage - 14);
                        SetEnragePercTB(CurrentPercentage - 14);

                        CurrentEnrageTime = 50;
                    }

                }
            });
        }
        public void ForceEnrageOff()
        {
            BossGage_EnragedUpdated(EntityId, false);
        }

        double ValueToLength(double value, double maxValue)
        {
            if (maxValue == 0)
            {
                return 0;
            }
            else
            {
                double n = BaseRect.ActualWidth * ((double)value / (double)maxValue);
                return n;
            }

        }

        static DoubleAnimation SlideAnimation = new DoubleAnimation();
        static ColorAnimation ColorChangeAnimation = new ColorAnimation();
        static DoubleAnimation DoubleAnimation = new DoubleAnimation();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HPrect.Fill = new SolidColorBrush(Color.FromArgb(0xff, 0x4a, 0x82, 0xbd));
            NextEnrage.RenderTransform = new TranslateTransform(BaseRect.Width * .86, 0);
            
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Boss.EnragedChanged -= BossGage_EnragedUpdated;
            //Boss.BossHPChanged -= BossGage_HPUpdated;

        }

        public void Reset()
        {
            Dispatcher.Invoke(() =>
            {
                CurrentHP = MaxHP;
                ForceEnrageOff();
                SetEnragePercTB(86);
                SlideNextEnrage(86);
            });
        }
    }
}
