using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Annotations;
using TCC.Controls;
using TCC.Data;
using TCC.ViewModels;

namespace TCC
{

    /// <summary>
    /// Logica di interazione per BossGage.xaml
    /// </summary>
    public partial class BossGage : UserControl, INotifyPropertyChanged
    {
        NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = ".", NumberDecimalDigits = 0 };
        readonly double barLength = 400;
        Color BaseHpColor = Color.FromRgb(0x00, 0x97, 0xce);
        public SynchronizedObservableCollection<EnragePeriodItem> EnrageHistory { get; set; }

        public string MainPercInt => (Convert.ToInt32(Math.Floor(Npc.CurrentFactor*100))).ToString();

        public string MainPercDec
        {
            get
            {
                double val = (Npc.CurrentFactor*100) %1 * 100;
                return $"{val:00}";

            }
        }
        public double AverageEnrage
        {
            get
            {
                var sum = 0D;
                if (EnrageHistory == null) return 0;
                if (EnrageHistory.Count == 0) return 0;
                foreach (var enragePeriodItem in EnrageHistory)
                {
                    sum += enragePeriodItem.Duration;
                }
                return sum / EnrageHistory.Count;
            }
        }
        public double TotalEnrage
        {
            get
            {
                var sum = 0D;
                if (EnrageHistory == null) return 0;
                if (EnrageHistory.Count == 0) return 0;
                foreach (var enragePeriodItem in EnrageHistory)
                {
                    sum += enragePeriodItem.Duration;
                }
                return sum;
            }
        }

        private Npc Npc => (Npc)DataContext;
        private float _maxHp;
        private float _currentHp;
        private bool _enraged;
        public double CurrentPercentage => _maxHp == 0 ? 0 : (_currentHp / _maxHp) * 100;
        private DoubleAnimation _shieldSizeAnim;
        private DoubleAnimation _enrageArcAnimation;
        private readonly DoubleAnimation _hpAnim;
        private readonly DoubleAnimation _flash;
        double nextEnragePerc;
        public double NextEnragePercentage
        {
            get => nextEnragePerc;
            set
            {
                if (nextEnragePerc != value)
                {
                    nextEnragePerc = value;
                    if (value < 0) nextEnragePerc = 0;
                    NotifyPropertyChanged("NextEnragePercentage");
                    NotifyPropertyChanged("EnrageTBtext");
                }
            }
        }

        public double RemainingPercentage => (CurrentPercentage - NextEnragePercentage) / Npc.EnragePattern.Percentage > 0 ? (CurrentPercentage - NextEnragePercentage) / Npc.EnragePattern.Percentage : 0;
        void NotifyPropertyChanged(string pr)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pr));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        int AnimationTime = 350;
        int curEnrageTime;
        public int CurrentEnrageTime
        {
            get => curEnrageTime;
            set
            {
                if (curEnrageTime != value)
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
                    return $"{CurrentEnrageTime}s";
                }
                else
                {
                    switch (SettingsManager.EnrageLabelMode)
                    {
                        case EnrageLabelMode.Next:
                            return $"{NextEnragePercentage:0.#}%";
                        case EnrageLabelMode.Remaining:
                            return $"{CurrentPercentage - NextEnragePercentage:0.#}%";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
            _hpAnim = new DoubleAnimation(1,TimeSpan.FromMilliseconds(150)){EasingFunction = new QuadraticEase()};
            _flash = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(1000)){EasingFunction = new QuadraticEase()};
            Timeline.SetDesiredFrameRate(_flash, 30);
            Timeline.SetDesiredFrameRate(_hpAnim, 30);
        }

        public bool ExtraInfo;
        private void boss_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentHP")
            {
                _currentHp = ((Npc)sender).CurrentHP;
                if (_currentHp > _maxHp) _maxHp = _currentHp;
                DoubleAnimation.To = ValueToLength(_currentHp, _maxHp);
                AnimateHp();
                NotifyPropertyChanged(nameof(EnrageTBtext));
                NotifyPropertyChanged(nameof(RemainingPercentage));
                //NotifyPropertyChanged(nameof(AverageEnrage));
                NotifyPropertyChanged(nameof(TotalEnrage));
                NotifyPropertyChanged(nameof(MainPercDec));
                NotifyPropertyChanged(nameof(MainPercInt));
                if (_enraged)
                {
                    SlideEnrageIndicator(CurrentPercentage);
                    EnrageHistory.Last().SetEnd(CurrentPercentage);
                    NotifyPropertyChanged(nameof(EnrageHistory));
                }
            }
            if (e.PropertyName == "MaxHP")
            {
                _maxHp = ((Npc)sender).MaxHP;
            }
            if (e.PropertyName == "Enraged")
            {
                var value = ((Npc)sender).Enraged;
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
                    EnrageHistory.Add(new EnragePeriodItem(CurrentPercentage));
                    NotifyPropertyChanged(nameof(EnrageHistory));
                    EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _enrageArcAnimation);
                    enrageBorder.BeginAnimation(OpacityProperty, _flash);
                }
                else
                {
                    NumberTimer?.Stop();

                    NextEnragePercentage = CurrentPercentage - Npc.EnragePattern.Percentage;
                    SlideEnrageIndicator(NextEnragePercentage);
                    EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                    ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = 0;
                    CurrentEnrageTime = Npc.EnragePattern.Duration;
                    NotifyPropertyChanged(nameof(RemainingPercentage));

                }
            }
            if (e.PropertyName == "Visible")
            {
                AnimateAppear();
            }
            if (e.PropertyName == nameof(Npc.ShieldFactor))
            {
                _shieldSizeAnim.To = Npc.ShieldFactor;
                //ShieldInnerFrameworkElement.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _shieldSizeAnim);
            }
        }

        private void AnimateAppear()
        {
            var sc = new ScaleTransform();
            sc.ScaleY = 0;
            LayoutTransform = sc;
            BossNameGrid.Opacity = 0;
            hpBarGrid.Opacity = 0;
            topInfoGrid.Opacity = 0;
            Visibility = Npc.Visible;
            var expand = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            Timeline.SetDesiredFrameRate(expand, 30);
            LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expand);
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            fade.BeginTime = TimeSpan.FromMilliseconds(300);
            Timeline.SetDesiredFrameRate(fade, 30);
            //mainBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, expand);
            mainBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expand);
            BossNameGrid.BeginAnimation(OpacityProperty, fade);
            hpBarGrid.BeginAnimation(OpacityProperty, fade);
            topInfoGrid.BeginAnimation(OpacityProperty, fade);
        }

        private void AnimateHp()
        {
            _hpAnim.To = Npc.CurrentFactor;
            DotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            HpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            Npc.PropertyChanged += boss_PropertyChanged;
            Npc.DeleteEvent += _boss_DeleteEvent;
            curEnrageTime = Npc.EnragePattern.Duration;
            _currentHp = Npc.CurrentHP;
            _maxHp = Npc.MaxHP;
            _enraged = Npc.Enraged;
            NextEnragePercentage = 100 - Npc.EnragePattern.Percentage;
            NextEnrage.RenderTransform = new TranslateTransform(hpBarGrid.Width, 0);
            SlideEnrageIndicator(NextEnragePercentage);
            _shieldSizeAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };
            _enrageArcAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(Npc.EnragePattern.Duration));
            Timeline.SetDesiredFrameRate(_enrageArcAnimation, 30);
            _enrageArcAnimation.Completed += _enrageArcAnimation_Completed;
            EnrageHistory = new SynchronizedObservableCollection<EnragePeriodItem>(Dispatcher);
            t = new DispatcherTimer() {Interval = TimeSpan.FromSeconds(5)};
            t.Tick += (s, ev) =>
            {
                t.Stop();
                var sc = new ScaleTransform();
                sc.ScaleY = 0;
                LayoutTransform = sc;

                var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));

                LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                    new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250)));
                BossNameGrid.BeginAnimation(OpacityProperty, fade);
                hpBarGrid.BeginAnimation(OpacityProperty, fade);
                topInfoGrid.BeginAnimation(OpacityProperty, fade);
                this.BeginAnimation(OpacityProperty, fade);

            };
            if (Npc.Visible == Visibility.Visible)
            {
                AnimateAppear();            
            }

        }

        private DispatcherTimer t;
        private void _boss_DeleteEvent()
        {
            NumberTimer?.Stop();
            NumberTimer?.Dispose();
            t.Start();
            Dispatcher.Invoke(() => BossGageWindowViewModel.Instance.RemoveMe(Npc));
        }

        private void _enrageArcAnimation_Completed(object sender, EventArgs e)
        {
            EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            try
            {
                ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = Npc.Enraged ? 1 : 0;
            }
            catch { }
        }

        void SlideEnrageIndicator(double val)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (val < 0)
                {
                    SlideAnimation.To = 0;
                }
                else
                {
                    SlideAnimation.To = hpBarGrid.ActualWidth * (val / 100);
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

        private void UIElement_OnMouseLeftButtonDown(object sender, RoutedEventArgs routedEventArgs)
        {
            ExtraInfo = !ExtraInfo;
            //ExtraBorder.Visibility = ExtraInfo ? Visibility.Visible: Visibility.Collapsed;
        }
    }

    public class EnragePeriodItem : TSPropertyChanged
    {
        public double Start { get; private set; }
        public double End { get; private set; }
        public double Factor => Duration * .01;
        public double StartFactor => End * .01;

        public double Duration => Start - End;
        public EnragePeriodItem(double start)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Start = start;

        }
        public void SetEnd(double end)
        {
            End = end;
            Refresh();
        }

        public void Refresh()
        {
            NotifyPropertyChanged(nameof(Factor));
            NotifyPropertyChanged(nameof(StartFactor));
        }
    }
}

namespace TCC.Converters
{
    public class EntityIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ulong) value == SessionManager.CurrentPlayer.EntityId
                ? SessionManager.CurrentPlayer.Name
                : (GroupWindowViewModel.Instance.TryGetUser((ulong) value, out var p) ? p.Name : "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AggroTypeToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AggroCircle x = (AggroCircle)value;

            switch (x)
            {
                case AggroCircle.Main:
                    return new SolidColorBrush(Colors.Orange);
                case AggroCircle.Secondary:
                    return new SolidColorBrush(Color.FromRgb(0x70, 0x40, 0xff));
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
                return new SolidColorBrush(Colors.DodgerBlue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
