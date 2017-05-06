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
        readonly double barLength = 400;
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

        public bool Enraged
        {
            get { return (bool)GetValue(EnragedProperty); }
            set { SetValue(EnragedProperty, value); }
        }
        public static readonly DependencyProperty EnragedProperty = DependencyProperty.Register("Enraged", typeof(bool), typeof(BossGage));

        public float CurrentPercentage
        {
            get
            {
                if(MaxHP > 0)
                {
                    return (CurrentHP / MaxHP);
                }
                else
                {
                    return 0;
                }
            }
        }

        float nextEnragePerc = 90;
        public float NextEnragePercentage
        {
            get => nextEnragePerc;
            set
            {
                if(nextEnragePerc != value)
                {
                    nextEnragePerc = value;
                    if (value < 0) nextEnragePerc = 0;
                    NotifyPropertyChanged("NextEnragePercentage");
                    NotifyPropertyChanged("EnrageTBtext");
                }
            }
        }

        void NotifyPropertyChanged(string pr)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pr));
        }

        int AnimationTime = 350;
        int EnrageDuration = 36000;
        int curEnrageTime = 36;
        public int CurrentEnrageTime
        {
            get => curEnrageTime;
            set
            {
                if(curEnrageTime != value)
                {
                    curEnrageTime = value;
                    NotifyPropertyChanged("CurrentEnrageTime");
                    NotifyPropertyChanged("EnrageTBtext");
                }
            }
        }
        public string EnrageTBtext
        {
            get
            {
                if (Enraged)
                {
                    return String.Format("{0}s", CurrentEnrageTime.ToString());
                }
                else
                {
                    return String.Format("{0:0.#}", NextEnragePercentage);
                }
            }
        }


        Timer NumberTimer = new Timer(1000);

        private DependencyPropertyWatcher<float> maxHPwatcher;
        private DependencyPropertyWatcher<float> currentHPwatcher;
        private DependencyPropertyWatcher<bool> enrageWatcher;

        public BossGage()
        {
            InitializeComponent();

            //Boss.EnragedChanged += BossGage_EnragedUpdated;
            //Boss.BossHPChanged += BossGage_HPUpdated;

            //BossNameTB.DataContext = this;
            //NextEnrageTB.DataContext = this;
            //AggroHolderNameTB.DataContext = this;
            
            //EnrageGrid.RenderTransform = new ScaleTransform(0, 1, 0, .5);

            SlideAnimation.EasingFunction = new QuadraticEase();
            ColorChangeAnimation.EasingFunction = new QuadraticEase();
            DoubleAnimation.EasingFunction = new QuadraticEase();
            SlideAnimation.Duration = TimeSpan.FromMilliseconds(250);
            ColorChangeAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
            DoubleAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);

            maxHPwatcher = new DependencyPropertyWatcher<float>(this, "MaxHP");
            currentHPwatcher = new DependencyPropertyWatcher<float>(this, "CurrentHP");
            enrageWatcher = new DependencyPropertyWatcher<bool>(this, "Enraged");

            maxHPwatcher.PropertyChanged += HPchanged;
            currentHPwatcher.PropertyChanged += HPchanged;
            enrageWatcher.PropertyChanged += EnrageChanged;
            //HPrect.RenderTransform = new ScaleTransform(1, 1,0,.5);

            //SetEnragePercTB(90);
        }

        private void EnrageChanged(object sender, EventArgs e)
        {
            if (!Enraged)
            {
                NextEnragePercentage = CurrentPercentage*100 - 10;
                NotifyPropertyChanged("Enraged");

                if (NumberTimer != null)
                {
                    NumberTimer.Stop();
                }

                SlideEnrageIndicator(NextEnragePercentage);

                NotifyPropertyChanged("EnrageTBtext");
                CurrentEnrageTime = 36;

            }
            else
            {
                SlideEnrageIndicator(CurrentPercentage*100);
                NumberTimer = new Timer(1000);
                NumberTimer.Elapsed += (s, ev) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CurrentEnrageTime--;
                    }));
                };
                NumberTimer.Enabled = true;
                NotifyPropertyChanged("Enraged");
                NotifyPropertyChanged("EnrageTBtext");
            }
        }

        private void HPchanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged("CurrentPercentage");
            if (CurrentHP > MaxHP)
            {
                MaxHP = CurrentHP;
            }
            if (Enraged)
            {
                SlideEnrageIndicator(CurrentPercentage*100);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //HPrect.Fill = new SolidColorBrush(BaseHpColor);
            Abnormalities.ItemsSource = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == EntityId).Buffs;
            NextEnrage.RenderTransform = new TranslateTransform(HPgauge.Width * .9, 0);
            //Perc2.Text = String.Format("{0} / {1}", CurrentHP.ToString("n", nfi), MaxHP.ToString("n", nfi));
        }

        //void SetEnragePercTB(double v)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        if (v < 0) v = 0;
        //        NextEnrageTB.Text = String.Format("{0:0.#}", v);
        //    });
        //}
        void SlideEnrageIndicator(double val)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if(val < 0)
                {
                    SlideAnimation.To = 0;
                }
                else
                {
                    SlideAnimation.To = HPgauge.ActualWidth * (val/100);
                }

                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, SlideAnimation);
            }));
        }
        //void GlowOn()
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        DoubleAnimation.To = 1;
        //        HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, DoubleAnimation);

        //        ColorChangeAnimation.To = Colors.Red;
        //        HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorChangeAnimation);
        //    }));
        //}
        //void GlowOff()
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        DoubleAnimation.To = 0;
        //        HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, DoubleAnimation);

        //        ColorChangeAnimation.To = BaseHpColor;
        //        HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ColorChangeAnimation);
        //    }));
        //}
        //void DeployEnrageGrid()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        DoubleAnimation.To = 1;
        //        //EnrageGrid.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimation);

        //        //EnrageArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(EnrageDuration)));

        //    });
        //}
        //void CloseEnrageGrid()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        DoubleAnimation.To = 0;
        //        //EnrageGrid.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimation);

        //    });
        //}

        private void BossGage_HPUpdated(ulong id, float hp)
        {      

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (id == EntityId)
                {
                    CurrentHP = hp;
                    if (CurrentHP > MaxHP)
                    {
                        MaxHP = CurrentHP;
                    }
                    //DoubleAnimation.To = ValueToLength(CurrentHP, MaxHP);
                    //HPrect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimation);

                    //Perc.Text = String.Format("{0:0.0}%", CurrentPercentage);
                    //Perc2.Text = String.Format("{0} / {1}", CurrentHP.ToString("n", nfi), MaxHP.ToString("n", nfi));

                    if (Enraged)
                    {
                        SlideEnrageIndicator(CurrentPercentage);
                        //SetEnragePercTB(CurrentPercentage);
                    }
                }
            }));
        }
        private void BossGage_EnragedUpdated(ulong id, bool enraged)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //if (id == this.EntityId)
                //{
                    //Enraged = enraged;
                    if (enraged)
                    {
                        //Enraged = enraged;
                        //number.Text = CurrentEnrageTime.ToString();
                        //NextEnrageTB.Text = CurrentEnrageTime.ToString();

                        SlideEnrageIndicator(CurrentPercentage);
                        //SetEnragePercTB(CurrentPercentage);

                        //GlowOn();
                        //DeployEnrageGrid();
                        NumberTimer = new Timer(1000);
                        NumberTimer.Elapsed += (s, ev) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //NextEnrageTB.Text = CurrentEnrageTime.ToString() + "s";
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

                        //GlowOff();
                        //CloseEnrageGrid();

                        SlideEnrageIndicator(CurrentPercentage - 10);
                        //SetEnragePercTB(CurrentPercentage - 10);

                        CurrentEnrageTime = 36;
                    }

        //        }
            }));
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
            //Boss.EnragedChanged -= BossGage_EnragedUpdated;
            //Boss.BossHPChanged -= BossGage_HPUpdated;

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

    public class BossHPbarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0x00, 0x97, 0xce));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
