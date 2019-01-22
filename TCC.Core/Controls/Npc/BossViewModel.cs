using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.NPCs;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Controls.NPCs
{
    public class BossViewModel : NpcViewModel
    {

        private double _nextEnragePerc;
        private bool _isTimerRunning;

        private readonly DispatcherTimer _numberTimer;

        public event Action EnragedChanged;

        public ObservableCollection<EnragePeriodItem> EnrageHistory { get; set; }
        public string MainPercInt => (Convert.ToInt32(Math.Floor(NPC.HPFactor * 100))).ToString();
        public string MainPercDec
        {
            get
            {
                double val = (NPC.HPFactor * 100) % 1 * 100;
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
        public double CurrentPercentage => NPC.HPFactor * 100;
        public double RemainingPercentage => (CurrentPercentage - NextEnragePercentage) / NPC.EnragePattern.Percentage > 0
                                           ? (CurrentPercentage - NextEnragePercentage) / NPC.EnragePattern.Percentage
                                           : 0;
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
                if (NPC.Enraged)
                {
                    return NPC.EnragePattern.StaysEnraged ? "∞" : $"{CurrentEnrageTime}s";
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

        public BossViewModel(NPC npc) : base(npc)
        {

            _numberTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _numberTimer.Tick += (_, __) =>
            {
                if (!NPC.EnragePattern.StaysEnraged)
                    CurrentEnrageTime--;
            };


            EnrageHistory = new SynchronizedObservableCollection<EnragePeriodItem>();

            NextEnragePercentage = 100 - NPC.EnragePattern.Percentage;
            CurrentEnrageTime = NPC.EnragePattern.StaysEnraged ? int.MaxValue : NPC.EnragePattern.Duration;


            NPC.PropertyChanged += OnPropertyChanged;
            NPC.DeleteEvent += () =>
            {
                BossGageWindowViewModel.Instance.RemoveMe(NPC, Delay + 250);
                _deleteTimer.Start();
                _numberTimer.Stop();
            };

            if (NPC.TimerPattern != null)
            {
                NPC.TimerPattern.Started += () => IsTimerRunning = true;
                NPC.TimerPattern.Ended += () => IsTimerRunning = false;
            }

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NPC.CurrentHP):
                    if (NPC.Enraged)
                    {
                        if (EnrageHistory.Count > 0) EnrageHistory.Last().SetEnd(CurrentPercentage);
                        N(nameof(EnrageHistory));
                    }
                    InvokeHpChanged();
                    N(nameof(EnrageTBtext));
                    N(nameof(RemainingPercentage));
                    N(nameof(TotalEnrage));
                    N(nameof(MainPercDec));
                    N(nameof(MainPercInt));
                    break;
                case nameof(NPC.MaxHP):
                    if (NPC.HPFactor == 1) NextEnragePercentage = 100 - NPC.EnragePattern.Percentage;
                    break;
                case nameof(NPC.Enraged):
                    if (NPC.Enraged)
                    {
                        EnrageHistory.Add(new EnragePeriodItem(CurrentPercentage));
                        //_numberTimer.Refresh();
                        N(nameof(EnrageHistory));
                    }
                    else
                    {
                        _numberTimer?.Stop();
                        NextEnragePercentage = CurrentPercentage - NPC.EnragePattern.Percentage;
                        CurrentEnrageTime = NPC.EnragePattern.StaysEnraged ? int.MaxValue : NPC.EnragePattern.Duration;
                        N(nameof(RemainingPercentage));
                    }
                    EnragedChanged?.Invoke();
                    break;
                case nameof(NPC.RemainingEnrageTime):
                    //_numberTimer.Refresh();
                    CurrentEnrageTime = NPC.RemainingEnrageTime / 1000;
                    N(nameof(EnrageTBtext));
                    N(nameof(RemainingPercentage));

                    break;

            }

        }
    }
}