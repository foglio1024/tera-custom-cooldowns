using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using TCC.Data;
using TCC.Parsing;

namespace TCC
{
    public class BuffDuration
    {
        public ulong Target { get; set; }
        public Abnormality Buff { get; set; }
        public int Duration { get; set; }
        public int Stacks { get; set; }

        public BuffDuration(Abnormality b, int d, int s, ulong t)
        {
            Buff = b;
            Duration = d;
            Stacks = s;
            Target = t;
        }
    }

    /// <summary>
    /// Logica di interazione per BossGage.xaml
    /// </summary>
    public partial class BossGage : UserControl, INotifyPropertyChanged
    {
        NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = ".", NumberDecimalDigits = 0 };

        public ulong EntityId
        {
            get { return (ulong)GetValue(EntityIdProperty); }
            set { SetValue(EntityIdProperty, value); }
        }
        public static readonly DependencyProperty EntityIdProperty = DependencyProperty.Register("EntityId", typeof(ulong), typeof(BossGage));

        public float MaxHP
        {
            get { return (float)GetValue(MaxHPProperty); }
            set { SetValue(MaxHPProperty, value); }
        }
        public static readonly DependencyProperty MaxHPProperty = DependencyProperty.Register("MaxHP", typeof(float), typeof(BossGage));

        public string BossName
        {
            get { return (string)GetValue(BossNameProperty); }
            set { SetValue(BossNameProperty, value); }
        }
        public static readonly DependencyProperty BossNameProperty = DependencyProperty.Register("BossName", typeof(string), typeof(BossGage));

        public float CurrentHP
        {
            get { return (float)GetValue(CurrentHPProperty); }
            set { SetValue(CurrentHPProperty, value); }
        }
        public static readonly DependencyProperty CurrentHPProperty =
            DependencyProperty.Register("CurrentHP", typeof(float), typeof(BossGage));


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
                        //Console.WriteLine("Last enrage percentage = {0} ({1}/{2})", LastEnragePercent, CurrentHP, MaxHP);
                    }
                    enraged = value;
                }
            }
        }

        //float currentHP;
        //public float CurrentHP
        //{
        //    get { return currentHP; }
        //    set
        //    {
        //        currentHP = value;
        //    }
        //}

        float CurrentPercentage
        {
            get
            {
                return (CurrentHP / MaxHP) * 100;
            }
        }
        void NotifyPropertyChanged(string pr)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pr));
        }

        int AnimationTime = 150;
        float LastEnragePercent = 100;
        int EnrageDuration = 36000;
        int CurrentEnrageTime = 36;

        Timer NumberTimer = new Timer(1000);

        public BossGage()
        {
            InitializeComponent();

            PacketRouter.EnragedChanged += BossGage_EnragedUpdated;
            PacketRouter.BossHPChanged += BossGage_HPUpdated;

            BossNameTB.DataContext = this;
            NextEnrageTB.DataContext = this;


            SlideAnimation.EasingFunction = new QuadraticEase();
            ColorChangeAnimation.EasingFunction = new QuadraticEase();
            DoubleAnimation.EasingFunction = new QuadraticEase();
            SlideAnimation.Duration = TimeSpan.FromMilliseconds(250);
            ColorChangeAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
            DoubleAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
            SetEnragePercTB(90);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HPrect.Fill = new SolidColorBrush(Color.FromArgb(0xff, 0x4a, 0x82, 0xbd));
            Abnormalities.ItemsSource = SessionManager.CurrentBosses.Where(x => x.EntityId == EntityId).First().Buffs;
            NextEnrage.RenderTransform = new TranslateTransform(BaseRect.Width * .9, 0);
            Perc2.Text = String.Format("{0} / {1}", CurrentHP.ToString("n", nfi), MaxHP.ToString("n", nfi));

        }

        void SetEnragePercTB(double v)
        {
            Dispatcher.Invoke(() =>
            {
                NextEnrageTB.Text = String.Format("{0:0.#}", v);
            });
        }
        void SlideNextEnrage(double val)
        {
            Dispatcher.Invoke(() =>
            {
                if(BaseRect.ActualWidth*(val/100) < 0)
                {
                    SlideAnimation.To = 0;
                }
                else
                {
                    SlideAnimation.To = BaseRect.ActualWidth * (val/100);
                }

                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, SlideAnimation);
            });
        }
        void SlideNextEnrageDirect()
        {
            Dispatcher.Invoke(() =>
            {
                SlideAnimation.To = HPrect.ActualWidth;
                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, SlideAnimation);
            });
        }
        void GlowOn()
        {
            Dispatcher.Invoke(() =>
            {
                DoubleAnimation.To = 1;
                HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, DoubleAnimation);

                ColorChangeAnimation.To = Colors.Red;
                HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorChangeAnimation);
            });
        }
        void GlowOff()
        {
            Dispatcher.Invoke(() =>
            {
                DoubleAnimation.To = 0;
                HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, DoubleAnimation);

                ColorChangeAnimation.To = Color.FromArgb(0xff, 0x4a, 0x82, 0xbd);
                HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorChangeAnimation);
            });
        }
        void DeployEnrageGrid()
        {
            Dispatcher.Invoke(() =>
            {
                DoubleAnimation.To = EnrageGrid.ActualHeight;
                EnrageGrid.BeginAnimation(WidthProperty, DoubleAnimation);

                EnrageArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(EnrageDuration)));

                NumberTimer = new Timer(1000);
                NumberTimer.Elapsed += (s, ev) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        number.Text = CurrentEnrageTime.ToString();
                        CurrentEnrageTime--;
                    }));
                };
                NumberTimer.Enabled = true;

            });
        }
        void CloseEnrageGrid()
        {
            Dispatcher.Invoke(() =>
            {
                DoubleAnimation.To = 0;
                EnrageGrid.BeginAnimation(WidthProperty, DoubleAnimation);

            });
        }

        private void BossGage_HPUpdated(ulong id, object hp)
        {      

            Dispatcher.BeginInvoke(new Action(() =>
                {
                    CurrentHP = Convert.ToInt32(hp);
                    if (id == EntityId)
                    {
                        DoubleAnimation.To = ValueToLength(CurrentHP, MaxHP);
                        HPrect.BeginAnimation(WidthProperty, DoubleAnimation);

                        Perc.Text = String.Format("{0:0.0}%", CurrentPercentage);
                        Perc2.Text = String.Format("{0} / {1}", CurrentHP.ToString("n", nfi), MaxHP.ToString("n", nfi));

                        if (Enraged)
                        {
                            SlideNextEnrage(CurrentPercentage);
                            SetEnragePercTB(CurrentPercentage);
                        }
                    }
                }));
        }
        private void BossGage_EnragedUpdated(ulong id, object enraged)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Enraged = (bool)enraged;

                if (id == this.EntityId)
                {
                    if ((bool)enraged)
                    {
                        Enraged = (bool)enraged;
                        number.Text = CurrentEnrageTime.ToString();

                        SlideNextEnrage(CurrentPercentage);
                        SetEnragePercTB(CurrentPercentage);

                        GlowOn();
                        DeployEnrageGrid();

                    }
                    else
                    {
                        if (NumberTimer != null)
                        {
                            NumberTimer.Stop();
                        }

                        GlowOff();
                        CloseEnrageGrid();

                        SlideNextEnrage(CurrentPercentage - 10);
                        SetEnragePercTB(CurrentPercentage - 10);

                        CurrentEnrageTime = 36;
                    }

                }
            }));
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
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
