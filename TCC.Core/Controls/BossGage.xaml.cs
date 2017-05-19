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
        public event PropertyChangedEventHandler PropertyChanged;

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


        public BossGage()
        {
            InitializeComponent();

            SlideAnimation.EasingFunction = new QuadraticEase();
            ColorChangeAnimation.EasingFunction = new QuadraticEase();
            DoubleAnimation.EasingFunction = new QuadraticEase();
            SlideAnimation.Duration = TimeSpan.FromMilliseconds(250);
            ColorChangeAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
            DoubleAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _boss = (Boss)DataContext;
            _boss.PropertyChanged += boss_PropertyChanged;

            _currentHp = _boss.CurrentHP;
            _maxHp = _boss.MaxHP;
            _enraged = _boss.Enraged;
            NextEnragePercentage = 90;
            NextEnrage.RenderTransform = new TranslateTransform(HPgauge.Width, 0);
            SlideEnrageIndicator(NextEnragePercentage);
        }

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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

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
