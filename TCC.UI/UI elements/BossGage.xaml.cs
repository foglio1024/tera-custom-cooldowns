using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using TCC.Data;
using TCC.Parsing;

namespace TCC
{

    /// <summary>
    /// Logica di interazione per BossGage.xaml
    /// </summary>
    public partial class BossGage : UserControl, INotifyPropertyChanged
    {
        NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = ".", NumberDecimalDigits = 0 };
        Color BaseHpColor = Color.FromRgb(0x00,0x97,0xce);
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
        public static readonly DependencyProperty CurrentHPProperty = DependencyProperty.Register("CurrentHP", typeof(float), typeof(BossGage));

        public ulong Target
        {
            get { return (ulong)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(ulong), typeof(BossGage));

        public AggroCircle CurrentAggroType
        {
            get { return (AggroCircle)GetValue(CurrentAggroTypeProperty); }
            set { SetValue(CurrentAggroTypeProperty, value); }
        }
        public static readonly DependencyProperty CurrentAggroTypeProperty = DependencyProperty.Register("CurrentAggroType", typeof(AggroCircle), typeof(BossGage));



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

        int AnimationTime = 350;
        float LastEnragePercent = 100;
        int EnrageDuration = 36000;
        int CurrentEnrageTime = 36;

        Timer NumberTimer = new Timer(1000);

        public BossGage()
        {
            InitializeComponent();

            Boss.EnragedChanged += BossGage_EnragedUpdated;
            Boss.BossHPChanged += BossGage_HPUpdated;

            BossNameTB.DataContext = this;
            NextEnrageTB.DataContext = this;
            AggroHolderNameTB.DataContext = this;
            
            //EnrageGrid.RenderTransform = new ScaleTransform(0, 1, 0, .5);

            SlideAnimation.EasingFunction = new QuadraticEase();
            ColorChangeAnimation.EasingFunction = new QuadraticEase();
            DoubleAnimation.EasingFunction = new QuadraticEase();
            SlideAnimation.Duration = TimeSpan.FromMilliseconds(250);
            ColorChangeAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
            DoubleAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);

            HPrect.RenderTransform = new ScaleTransform(1, 1,0,.5);

            SetEnragePercTB(90);

            Console.WriteLine("Boss gauge created: {0}", BossName);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HPrect.Fill = new SolidColorBrush(BaseHpColor);
            Abnormalities.ItemsSource = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == EntityId).Buffs;
            NextEnrage.RenderTransform = new TranslateTransform(BaseRect.Width * .9, 0);
            Perc2.Text = String.Format("{0} / {1}", CurrentHP.ToString("n", nfi), MaxHP.ToString("n", nfi));
            Console.WriteLine("Boss gauge loaded: {0}", BossName);


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
                if(val < 0)
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

                ColorChangeAnimation.To = BaseHpColor;
                HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorChangeAnimation);
            });
        }
        void DeployEnrageGrid()
        {
            Dispatcher.Invoke(() =>
            {
                DoubleAnimation.To = 1;
                //EnrageGrid.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimation);

                //EnrageArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(EnrageDuration)));

            });
        }
        void CloseEnrageGrid()
        {
            Dispatcher.Invoke(() =>
            {
                DoubleAnimation.To = 0;
                //EnrageGrid.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimation);

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
                    HPrect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimation);

                    Perc.Text = String.Format("{0:0.0}%", CurrentPercentage);
                    Perc2.Text = String.Format("{0} / {1}", CurrentHP.ToString("n", nfi), MaxHP.ToString("n", nfi));

                    if (Enraged)
                    {
                        SlideNextEnrage(CurrentPercentage);
                        //SetEnragePercTB(CurrentPercentage);
                    }
                }
            });
        }
        private void BossGage_EnragedUpdated(ulong id, bool enraged)
        {
            Dispatcher.Invoke(() =>
            {
                if (id == this.EntityId)
                {
                    Enraged = enraged;
                    if (enraged)
                    {
                        Enraged = enraged;
                        //number.Text = CurrentEnrageTime.ToString();
                        NextEnrageTB.Text = CurrentEnrageTime.ToString();

                        SlideNextEnrage(CurrentPercentage);
                        //SetEnragePercTB(CurrentPercentage);

                        GlowOn();
                        //DeployEnrageGrid();
                        NumberTimer = new Timer(1000);
                        NumberTimer.Elapsed += (s, ev) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                NextEnrageTB.Text = CurrentEnrageTime.ToString();
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
                        //CloseEnrageGrid();

                        SlideNextEnrage(CurrentPercentage - 10);
                        SetEnragePercTB(CurrentPercentage - 10);

                        CurrentEnrageTime = 36;
                    }

                }
            });
        }
        double ValueToLength(double value, double maxValue)
        {
            if (maxValue == 0)
            {
                return 1;
            }
            else
            {
                double n = ((double)value / (double)maxValue);
                return n;
            }

        }

        static DoubleAnimation SlideAnimation = new DoubleAnimation();
        static ColorAnimation ColorChangeAnimation = new ColorAnimation();
        static DoubleAnimation DoubleAnimation = new DoubleAnimation();
        public event PropertyChangedEventHandler PropertyChanged;

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Boss.EnragedChanged -= BossGage_EnragedUpdated;
            Boss.BossHPChanged -= BossGage_HPUpdated;

        }
    }

}

namespace TCC.Converters
{
    public class EntityIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((ulong)value == SessionManager.CurrentPlayer.EntityId)
            {
                return SessionManager.CurrentPlayer.Name;
            }
            else
            {
                if(EntitiesManager.TryGetUserById((ulong)value, out Player p))
                {
                    return p.Name;
                }
                else
                {
                    return "";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AggroTypeToFill : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AggroCircle x = (AggroCircle)value;

            switch (x)
            {
                case AggroCircle.Main:
                    return new SolidColorBrush(Colors.Orange);
                case AggroCircle.Secondary:
                    return new SolidColorBrush(Color.FromRgb(0x70,0x40,0xff));
                case AggroCircle.None:
                    return new SolidColorBrush(Colors.Transparent);
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
