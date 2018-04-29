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
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Logica di interazione per BossGage.xaml
    /// </summary>
    public partial class BossGage : INotifyPropertyChanged
    {
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
        private double _nextEnragePerc;
        public double NextEnragePercentage
        {
            get => _nextEnragePerc;
            set
            {
                if (_nextEnragePerc != value)
                {
                    _nextEnragePerc = value;
                    if (value < 0) _nextEnragePerc = 0;
                    NotifyPropertyChanged(nameof(NextEnragePercentage));
                    NotifyPropertyChanged(nameof(EnrageTBtext));
                }
            }
        }

        public double RemainingPercentage => (CurrentPercentage - NextEnragePercentage) / Npc.EnragePattern.Percentage > 0 ? (CurrentPercentage - NextEnragePercentage) / Npc.EnragePattern.Percentage : 0;

        private void NotifyPropertyChanged(string pr)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pr));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private const int AnimationTime = 350;
        private int _curEnrageTime;
        public int CurrentEnrageTime
        {
            get => _curEnrageTime;
            set
            {
                if (_curEnrageTime != value)
                {
                    _curEnrageTime = value;
                    NotifyPropertyChanged(nameof(CurrentEnrageTime));
                    NotifyPropertyChanged(nameof(EnrageTBtext));
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

        private Timer _numberTimer = new Timer(1000);

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

        private void Boss_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentHP":
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
                        if(EnrageHistory.Count > 0) EnrageHistory.Last().SetEnd(CurrentPercentage);
                        NotifyPropertyChanged(nameof(EnrageHistory));
                    }

                    break;
                case "MaxHP":
                    _maxHp = ((Npc)sender).MaxHP;
                    break;
                case "Enraged":
                    var value = ((Npc)sender).Enraged;
                    if (_enraged == value) return;
                    _enraged = value;
                    if (_enraged)
                    {
                        SlideEnrageIndicator(CurrentPercentage);
                        _numberTimer = new Timer(1000);
                        _numberTimer.Elapsed += (s, ev) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                CurrentEnrageTime--;
                            }));
                        };
                        _numberTimer.Enabled = true;
                        EnrageHistory.Add(new EnragePeriodItem(CurrentPercentage));
                        NotifyPropertyChanged(nameof(EnrageHistory));
                        EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _enrageArcAnimation);
                        EnrageBorder.BeginAnimation(OpacityProperty, _flash);
                    }
                    else
                    {
                        _numberTimer?.Stop();

                        NextEnragePercentage = CurrentPercentage - Npc.EnragePattern.Percentage;
                        SlideEnrageIndicator(NextEnragePercentage);
                        EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                        ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = 0;
                        CurrentEnrageTime = Npc.EnragePattern.Duration;
                        NotifyPropertyChanged(nameof(RemainingPercentage));

                    }
                    break;
                case "Visible":
                    AnimateAppear();
                    break;
                case nameof(Npc.ShieldFactor):
                    _shieldSizeAnim.To = Npc.ShieldFactor;
                    //ShieldInnerFrameworkElement.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _shieldSizeAnim);
                    break;
            }
        }

        private void AnimateAppear()
        {
            var sc = new ScaleTransform {ScaleY = 0};
            LayoutTransform = sc;
            BossNameGrid.Opacity = 0;
            HpBarGrid.Opacity = 0;
            TopInfoGrid.Opacity = 0;
            Visibility = Npc.Visible;
            var expand = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            Timeline.SetDesiredFrameRate(expand, 30);
            LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expand);
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromMilliseconds(300)
            };
            Timeline.SetDesiredFrameRate(fade, 30);
            //mainBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, expand);
            MainBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expand);
            BossNameGrid.BeginAnimation(OpacityProperty, fade);
            HpBarGrid.BeginAnimation(OpacityProperty, fade);
            TopInfoGrid.BeginAnimation(OpacityProperty, fade);
        }

        private void AnimateHp()
        {
            // ReSharper disable once IsExpressionAlwaysTrue
            if (!(Npc is Npc)) return; //weird but could happen
            _hpAnim.To = Npc.CurrentFactor;
            DotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            HpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            Npc.PropertyChanged += Boss_PropertyChanged;
            Npc.DeleteEvent += _boss_DeleteEvent;
            _curEnrageTime = Npc.EnragePattern.Duration;
            _currentHp = Npc.CurrentHP;
            _maxHp = Npc.MaxHP;
            _enraged = Npc.Enraged;
            NextEnragePercentage = 100 - Npc.EnragePattern.Percentage;
            NextEnrage.RenderTransform = new TranslateTransform(HpBarGrid.Width, 0);
            SlideEnrageIndicator(NextEnragePercentage);
            _shieldSizeAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };
            _enrageArcAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(Npc.EnragePattern.Duration));
            Timeline.SetDesiredFrameRate(_enrageArcAnimation, 30);
            _enrageArcAnimation.Completed += _enrageArcAnimation_Completed;
            EnrageHistory = new SynchronizedObservableCollection<EnragePeriodItem>(Dispatcher);
            _t = new DispatcherTimer() {Interval = TimeSpan.FromSeconds(5)};
            _t.Tick += (s, ev) =>
            {
                _t.Stop();
                var sc = new ScaleTransform {ScaleY = 0};
                LayoutTransform = sc;

                var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));

                LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                    new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250)));
                BossNameGrid.BeginAnimation(OpacityProperty, fade);
                HpBarGrid.BeginAnimation(OpacityProperty, fade);
                TopInfoGrid.BeginAnimation(OpacityProperty, fade);
                BeginAnimation(OpacityProperty, fade);

            };
            if (Npc.Visible == Visibility.Visible)
            {
                AnimateAppear();            
            }

        }

        private DispatcherTimer _t;
        private void _boss_DeleteEvent()
        {
            _numberTimer?.Stop();
            _numberTimer?.Dispose();
            _t.Start();
            try
            {
                Dispatcher.Invoke(() => BossGageWindowViewModel.Instance.RemoveMe(Npc));
            }
            catch
            {
                // ignored
            }
        }

        private void _enrageArcAnimation_Completed(object sender, EventArgs e)
        {
            EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            try
            {
                ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = Npc.Enraged ? 1 : 0;
            }
            catch
            {
                // ignored
            }
        }

        private void SlideEnrageIndicator(double val)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (val < 0)
                {
                    SlideAnimation.To = 0;
                }
                else
                {
                    SlideAnimation.To = HpBarGrid.ActualWidth * (val / 100);
                }

                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, SlideAnimation);
            }));
        }

        private static double ValueToLength(double value, double maxValue)
        {
            if (maxValue == 0) return 1;
            var n = value / maxValue;
            return n;

        }

        private static readonly DoubleAnimation SlideAnimation = new DoubleAnimation();
        private static readonly ColorAnimation ColorChangeAnimation = new ColorAnimation();
        private static readonly DoubleAnimation DoubleAnimation = new DoubleAnimation();

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public class EnragePeriodItem : TSPropertyChanged
    {
        public double Start { get; }
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

        private void Refresh()
        {
            NPC(nameof(Factor));
            NPC(nameof(StartFactor));
        }
    }
}

namespace TCC.Converters
{
    public class EntityIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
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
            // ReSharper disable once PossibleNullReferenceException
            var x = (AggroCircle)value;

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
            // ReSharper disable once PossibleNullReferenceException
            return (bool) value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.DodgerBlue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
