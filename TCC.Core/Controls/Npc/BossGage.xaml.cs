using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.NPCs;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Controls.NPCs
{
    public class BossViewModel : TSPropertyChanged
    {
        public const uint Delay = 5000;

        private double _nextEnragePerc;
        private bool _isTimerRunning;

        private readonly DispatcherTimer _numberTimer;
        private readonly DispatcherTimer _deleteTimer;

        public event Action HpFactorChanged;
        public event Action EnragedChanged;
        public event Action Disposed;

        public NPC Boss { get; set; }

        public SynchronizedObservableCollection<EnragePeriodItem> EnrageHistory { get; set; }
        public string MainPercInt => (Convert.ToInt32(Math.Floor(Boss.HPFactor * 100))).ToString();
        public string MainPercDec
        {
            get
            {
                double val = (Boss.HPFactor * 100) % 1 * 100;
                val = val > 99 ? 99 : val;
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
        public double CurrentPercentage => Boss.HPFactor * 100;
        public double RemainingPercentage => (CurrentPercentage - NextEnragePercentage) / Boss.EnragePattern.Percentage > 0 ? (CurrentPercentage - NextEnragePercentage) / Boss.EnragePattern.Percentage : 0;
        public double NextEnragePercentage
        {
            get => _nextEnragePerc;
            set
            {
                if (_nextEnragePerc != value)
                {
                    _nextEnragePerc = value;
                    if (value < 0) _nextEnragePerc = 0;
                    N();
                    N(nameof(EnrageTBtext));
                }
            }
        }
        public string EnrageTBtext
        {
            get
            {
                if (Boss.Enraged)
                {
                    return Boss.EnragePattern.StaysEnraged ? "∞" : $"{CurrentEnrageTime}s";
                }
                else
                {
                    switch (SettingsHolder.EnrageLabelMode)
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
        private int _curEnrageTime;
        public int CurrentEnrageTime
        {
            get => _curEnrageTime;
            set
            {
                if (_curEnrageTime != value)
                {
                    _curEnrageTime = value;
                    N();
                    N(nameof(EnrageTBtext));
                }
            }
        }

        public bool IsTimerRunning
        {
            get => _isTimerRunning;
            set
            {
                if (_isTimerRunning == value) return;
                _isTimerRunning = value;
                N();
            }
        }

        public BossViewModel(NPC npc)
        {
            Boss = npc;

            _numberTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _numberTimer.Tick += (_, __) => { if (!Boss.EnragePattern.StaysEnraged) CurrentEnrageTime--; };

            _deleteTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Delay) };
            _deleteTimer.Tick += (s, ev) =>
            {
                _deleteTimer.Stop();
                Disposed?.Invoke();
            };
            EnrageHistory = new SynchronizedObservableCollection<EnragePeriodItem>();

            NextEnragePercentage = 100 - Boss.EnragePattern.Percentage;

            Boss.PropertyChanged += OnPropertyChanged;
            Boss.DeleteEvent += () =>
            {
                _deleteTimer.Start();
                _numberTimer.Stop();
            };

            if (Boss.TimerPattern != null)
            {
                Boss.TimerPattern.Started += () => IsTimerRunning = true;
                Boss.TimerPattern.Ended += () => IsTimerRunning = false;
            }

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NPC.CurrentHP):
                    if (Boss.Enraged)
                    {
                        if (EnrageHistory.Count > 0) EnrageHistory.Last().SetEnd(CurrentPercentage);
                        N(nameof(EnrageHistory));
                    }
                    HpFactorChanged?.Invoke();
                    N(nameof(EnrageTBtext));
                    N(nameof(RemainingPercentage));
                    N(nameof(TotalEnrage));
                    N(nameof(MainPercDec));
                    N(nameof(MainPercInt));
                    break;
                case nameof(NPC.MaxHP):
                    if (Boss.HPFactor == 1) NextEnragePercentage = 100 - Boss.EnragePattern.Percentage;
                    break;
                case nameof(NPC.Enraged):
                    EnragedChanged?.Invoke();
                    if (Boss.Enraged)
                    {
                        EnrageHistory.Add(new EnragePeriodItem(CurrentPercentage));
                        _numberTimer.Refresh();
                        N(nameof(EnrageHistory));
                    }
                    else
                    {
                        _numberTimer?.Stop();
                        NextEnragePercentage = CurrentPercentage - Boss.EnragePattern.Percentage;
                        CurrentEnrageTime = Boss.EnragePattern.StaysEnraged ? int.MaxValue : Boss.EnragePattern.Duration;
                        N(nameof(RemainingPercentage));

                    }
                    break;

            }

        }
    }
    public partial class BossGage
    {
        public BossViewModel VM { get; set; }

        private readonly DoubleAnimation _enrageArcAnimation;
        private readonly DoubleAnimation _hpAnim;
        private readonly DoubleAnimation _flash;
        private readonly DoubleAnimation _slideAnim;
        private readonly DoubleAnimation _timerAnim;
        private readonly DoubleAnimation _fadeAnim;

        public BossGage()
        {

            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is NPC npc) VM = new BossViewModel(npc);
            };

            _slideAnim = new DoubleAnimation
            {
                EasingFunction = R.MiscResources.QuadraticEase,
                Duration = TimeSpan.FromMilliseconds(250)
            };
            Timeline.SetDesiredFrameRate(_slideAnim, 60);

            _hpAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150))
            {
                EasingFunction = R.MiscResources.QuadraticEase,
            };
            Timeline.SetDesiredFrameRate(_hpAnim, 60);

            _flash = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = R.MiscResources.QuadraticEase,
            };
            Timeline.SetDesiredFrameRate(_flash, 30);

            _timerAnim = new DoubleAnimation { To = 0 };
            Timeline.SetDesiredFrameRate(_timerAnim, 20);

            _enrageArcAnimation = new DoubleAnimation { From = 1, To = 0 };
            _enrageArcAnimation.Completed += _enrageArcAnimation_Completed;
            Timeline.SetDesiredFrameRate(_enrageArcAnimation, 30);

            _fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));
            Timeline.SetDesiredFrameRate(_fadeAnim, 30);

            SettingsWindowViewModel.AbnormalityShapeChanged += RefreshAbnormalityTemplate;


        }

        private void RefreshAbnormalityTemplate()
        {
            Abnormalities.ItemTemplateSelector = null;
            Abnormalities.ItemTemplateSelector = R.TemplateSelectors.BossAbnormalityTemplateSelector;
        }

        private void AnimateTimer()
        {
            Dispatcher.Invoke(() =>
            {
                _timerAnim.From = VM.Boss.TimerPattern is HpTriggeredTimerPattern hptp ? hptp.StartAt : 1;
                _timerAnim.Duration = TimeSpan.FromSeconds(VM.Boss.TimerPattern.Duration);
                TimerDotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
                TimerBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
            });
        }
        private void OnHpChanged()
        {
            //_doubleAnim.To = DC.Boss.HPFactor;
            AnimateHp();
            if (VM.Boss.Enraged) SlideEnrageIndicator(VM.CurrentPercentage);
        }
        private void OnEnragedChanged()
        {
            if (VM.Boss.Enraged)
            {
                SlideEnrageIndicator(VM.CurrentPercentage);
                EnrageBorder.BeginAnimation(OpacityProperty, _flash);
                if (!VM.Boss.EnragePattern.StaysEnraged) EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _enrageArcAnimation);
            }
            else
            {
                SlideEnrageIndicator(VM.NextEnragePercentage);
                EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = 0;

            }

        }

        private void AnimateHp()
        {
            if (VM.Boss == null) return; //weird but could happen 
            _hpAnim.To = VM.Boss.HPFactor; //still crashing here ffs
            DotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            HpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            VM.HpFactorChanged += OnHpChanged;
            VM.EnragedChanged += OnEnragedChanged;
            VM.Boss.DeleteEvent += StartDeletion;
            VM.Disposed += AnimateFadeOut;
            if (VM.Boss.TimerPattern != null) VM.Boss.TimerPattern.Started += AnimateTimer;

            NextEnrage.RenderTransform = new TranslateTransform(HpBarGrid.Width, 0);

            SlideEnrageIndicator(VM.NextEnragePercentage);

            _enrageArcAnimation.Duration = TimeSpan.FromSeconds(VM.Boss.EnragePattern.Duration);

        }

        private void AnimateFadeOut()
        {
            LayoutTransform = new ScaleTransform { ScaleY = 0 };
            LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _fadeAnim);

            BossNameGrid.BeginAnimation(OpacityProperty, _fadeAnim);
            HpBarGrid.BeginAnimation(OpacityProperty, _fadeAnim);
            TopInfoGrid.BeginAnimation(OpacityProperty, _fadeAnim);
            BeginAnimation(OpacityProperty, _fadeAnim);
        }

        private void StartDeletion()
        {
            try
            {
                SettingsWindowViewModel.AbnormalityShapeChanged -= RefreshAbnormalityTemplate;
                Dispatcher.Invoke(() => BossGageWindowViewModel.Instance.RemoveMe(VM.Boss, BossViewModel.Delay + 250));
            }
            catch { /*ignored*/ }
        }

        private void _enrageArcAnimation_Completed(object sender, EventArgs e)
        {
            EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            try
            {
                ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = VM.Boss.Enraged ? 1 : 0;
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
                _slideAnim.To = val < 0 ? 0 : HpBarGrid.ActualWidth * (val / 100);
                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideAnim);
            }));
        }

    }
}