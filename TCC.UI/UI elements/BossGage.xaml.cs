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

        private Boss _boss;
        private float _maxHp;
        private float _currentHp;
        private bool _enraged;
        public float CurrentPercentage => _maxHp == 0 ? 0 : (_currentHp / _maxHp) * 100;

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
                if (_enraged)
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

        //private DependencyPropertyWatcher<float> maxHPwatcher;
        //private DependencyPropertyWatcher<float> currentHPwatcher;
        //private DependencyPropertyWatcher<bool> enrageWatcher;

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

            //maxHPwatcher = new DependencyPropertyWatcher<float>(this, "MaxHP");
            //currentHPwatcher = new DependencyPropertyWatcher<float>(this, "CurrentHP");
            //enrageWatcher = new DependencyPropertyWatcher<bool>(this, "Enraged");

            //maxHPwatcher.PropertyChanged += HPchanged;
            //currentHPwatcher.PropertyChanged += HPchanged;
            //enrageWatcher.PropertyChanged += EnrageChanged;
            //HPrect.RenderTransform = new ScaleTransform(1, 1,0,.5);

            //SetEnragePercTB(90);
        }

        private void boss_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentHP")
            {
                _currentHp = ((Boss)sender).CurrentHP;
                if (_currentHp > _maxHp) _maxHp = _currentHp;
                DoubleAnimation.To = ValueToLength(_currentHp, _maxHp);

                if (_enraged)
                {
                    SlideEnrageIndicator(CurrentPercentage);
                }
            }
            if (e.PropertyName == "MaxHP")
            {
                _maxHp = ((Boss)sender).MaxHP;
            }
            if (e.PropertyName == "Enraged")
            {
                var value = ((Boss)sender).Enraged;
                if (_enraged == value) return;
                _enraged = value;
                if (_enraged)
                {
                    SlideEnrageIndicator(CurrentPercentage);
                    NumberTimer = new Timer(1000);
                    NumberTimer.Elapsed += (s, ev) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            CurrentEnrageTime--;
                        }));
                    };
                    NumberTimer.Enabled = true;
                }
                else
                {
                    NumberTimer?.Stop();

                    NextEnragePercentage = CurrentPercentage - 10;
                    SlideEnrageIndicator(NextEnragePercentage);

                    CurrentEnrageTime = 36;
                }
            }
            if (e.PropertyName == "Visible")
            {
                Visibility = ((Boss)sender).Visible;
            }
        }

        //private void EnrageChanged(object sender, EventArgs e)
        //{
        //    if (!Enraged)
        //    {
        //        NextEnragePercentage = CurrentPercentage*100 - 10;
        //        NotifyPropertyChanged("Enraged");

        //        if (NumberTimer != null)
        //        {
        //            NumberTimer.Stop();
        //        }

        //        SlideEnrageIndicator(NextEnragePercentage);

        //        NotifyPropertyChanged("EnrageTBtext");
        //        CurrentEnrageTime = 36;

        //    }
        //    else
        //    {
        //        SlideEnrageIndicator(CurrentPercentage*100);
        //        NumberTimer = new Timer(1000);
        //        NumberTimer.Elapsed += (s, ev) =>
        //        {
        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                CurrentEnrageTime--;
        //            }));
        //        };
        //        NumberTimer.Enabled = true;
        //        NotifyPropertyChanged("Enraged");
        //        NotifyPropertyChanged("EnrageTBtext");
        //    }
        //}

        //private void HPchanged(object sender, EventArgs e)
        //{
        //    NotifyPropertyChanged("CurrentPercentage");
        //    if (CurrentHP > MaxHP)
        //    {
        //        MaxHP = CurrentHP;
        //    }
        //    if (Enraged)
        //    {
        //        SlideEnrageIndicator(CurrentPercentage*100);
        //    }
        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _boss = (Boss)DataContext;
            _boss.PropertyChanged += boss_PropertyChanged;

            _currentHp = _boss.CurrentHP;
            _maxHp = _boss.MaxHP;
            _enraged = _boss.Enraged;
            NextEnragePercentage = CurrentPercentage - 10;
            SlideEnrageIndicator(NextEnragePercentage);

            //HPrect.Fill = new SolidColorBrush(BaseHpColor);
            //Abnormalities.ItemsSource = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == EntityId).Buffs;
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

        //private void BossGage_HPUpdated(ulong id, float hp)
        //{      

        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        if (id == EntityId)
        //        {
        //            CurrentHP = hp;
        //            if (CurrentHP > MaxHP)
        //            {
        //                MaxHP = CurrentHP;
        //            }
        //            //DoubleAnimation.To = ValueToLength(CurrentHP, MaxHP);
        //            //HPrect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimation);

        //            //Perc.Text = String.Format("{0:0.0}%", CurrentPercentage);
        //            //Perc2.Text = String.Format("{0} / {1}", CurrentHP.ToString("n", nfi), MaxHP.ToString("n", nfi));

        //            if (Enraged)
        //            {
        //                SlideEnrageIndicator(CurrentPercentage);
        //                //SetEnragePercTB(CurrentPercentage);
        //            }
        //        }
        //    }));
        //}
        //private void BossGage_EnragedUpdated(ulong id, bool enraged)
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        //if (id == this.EntityId)
        //        //{
        //            //Enraged = enraged;
        //            if (enraged)
        //            {
        //                //Enraged = enraged;
        //                //number.Text = CurrentEnrageTime.ToString();
        //                //NextEnrageTB.Text = CurrentEnrageTime.ToString();

        //                SlideEnrageIndicator(CurrentPercentage);
        //                //SetEnragePercTB(CurrentPercentage);

        //                //GlowOn();
        //                //DeployEnrageGrid();
        //                NumberTimer = new Timer(1000);
        //                NumberTimer.Elapsed += (s, ev) =>
        //                {
        //                    Dispatcher.BeginInvoke(new Action(() =>
        //                    {
        //                        //NextEnrageTB.Text = CurrentEnrageTime.ToString() + "s";
        //                        CurrentEnrageTime--;
        //                    }));
        //                };
        //                NumberTimer.Enabled = true;
        //            }
        //            else
        //            {
        //                if (NumberTimer != null)
        //                {
        //                    NumberTimer.Stop();
        //                }

        //                //GlowOff();
        //                //CloseEnrageGrid();

        //                SlideEnrageIndicator(CurrentPercentage - 10);
        //                //SetEnragePercTB(CurrentPercentage - 10);

        //                CurrentEnrageTime = 36;
        //            }

        ////        }
        //    }));
        //}
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
