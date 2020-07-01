using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using Nostrum;
using Nostrum.Extensions;
using TCC.Data;

namespace TCC.UI.Controls.NPCs
{
    public class BossViewModel : NpcViewModel
    {

        private double _nextEnragePerc;
        private bool _isTimerRunning;
        private bool _serverSentEnrage;

        private readonly DispatcherTimer _numberTimer;

        public event Action EnragedChanged = null!;
        public event Action ReEnraged = null!;

        public ObservableCollection<EnragePeriodItem> EnrageHistory { get; set; }
        public string MainPercInt => ShowHP ? Convert.ToInt32(Math.Floor(NPC.HPFactor * 100)).ToString() : "?";
        public string MainPercDec
        {
            get
            {
                if (!ShowHP) return "??";
                double val = NPC.HPFactor * 100 % 1 * 100;
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
            private set
            {
                if (_nextEnragePerc == value) return;
                _nextEnragePerc = value;
                if (value < 0) _nextEnragePerc = 0;
                N();
                N(nameof(EnrageTBtext));
            }
        }
        public string EnrageTBtext
        {
            get
            {
                if (NPC.Enraged)
                {
                    return NPC.EnragePattern.StaysEnraged ? "∞" : $"{TimeUtils.FormatTime(CurrentEnrageTime)}";
                }
                else
                {
                    return App.Settings.NpcWindowSettings.EnrageLabelMode switch
                    {
                        EnrageLabelMode.Next => $"{NextEnragePercentage:0.#}%",
                        EnrageLabelMode.Remaining => $"{CurrentPercentage - NextEnragePercentage:0.#}%",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }
        }
        public string RhombEnrageTimerText
        {
            get
            {
                return NPC.Enraged
                        ? NPC.EnragePattern.StaysEnraged
                            ? "∞"
                            : NPC.EnragePattern.Duration != 0
                                ? $"{TimeUtils.FormatTime(CurrentEnrageTime)}"
                                : "-"
                        : "";
            }
        }
        private int _curEnrageTime;
        public int CurrentEnrageTime
        {
            get => _curEnrageTime;
            set
            {
                if (_curEnrageTime == value) return;
                _curEnrageTime = value;
                N();
                N(nameof(EnrageTBtext));
                N(nameof(RhombEnrageTimerText));
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

        public BossViewModel(Data.NPCs.NPC npc) : base(npc)
        {

            _numberTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _numberTimer.Tick += OnNumberTimerTick;


            EnrageHistory = new TSObservableCollection<EnragePeriodItem>();

            NextEnragePercentage = 100 - NPC.EnragePattern.Percentage;
            CurrentEnrageTime = NPC.EnragePattern.StaysEnraged ? int.MaxValue : NPC.EnragePattern.Duration;


            NPC.PropertyChanged += OnPropertyChanged;

            if (NPC.TimerPattern == null) return;
            NPC.TimerPattern.Started += OnTimerPatternStarted;
            NPC.TimerPattern.Ended += OnTimerPatternEnded;

        }

        private void OnTimerPatternEnded()
        {
            IsTimerRunning = false;
        }

        private void OnTimerPatternStarted()
        {
            IsTimerRunning = true;
        }

        private void OnNumberTimerTick(object? _, EventArgs __)
        {
            if (!NPC.EnragePattern.StaysEnraged) CurrentEnrageTime--;
        }

        protected override void OnNpcDelete()
        {
            WindowManager.ViewModels.NpcVM.RemoveNPC(NPC, Delay + 250);
            _numberTimer.Stop();
            _numberTimer.Tick -= OnNumberTimerTick;
            NPC.PropertyChanged -= OnPropertyChanged;
            if (NPC.TimerPattern != null)
            {
                NPC.TimerPattern.Started -= OnTimerPatternStarted;
                NPC.TimerPattern.Ended -= OnTimerPatternEnded;
            }

            base.OnNpcDelete();
        }

        private bool _addEnrageItem = true;
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NPC.CurrentHP):
                    if (NPC.Enraged)
                    {
                        if (_addEnrageItem)
                        {
                            EnrageHistory.Add(new EnragePeriodItem(CurrentPercentage));
                            _addEnrageItem = false;
                        }
                        if (EnrageHistory.Count > 0)
                        {
                            EnrageHistory.Last().SetEnd(CurrentPercentage);
                            //N(nameof(EnrageHistory));
                        }
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
                        _addEnrageItem = false;
                        _numberTimer.Refresh();
                        N(nameof(RhombEnrageTimerText));
                        //N(nameof(EnrageHistory));
                    }
                    else
                    {
                        _addEnrageItem = true;
                        _serverSentEnrage = false;
                        _numberTimer?.Stop();
                        NextEnragePercentage = CurrentPercentage - NPC.EnragePattern.Percentage;
                        CurrentEnrageTime = NPC.EnragePattern.StaysEnraged ? int.MaxValue : NPC.EnragePattern.Duration;
                        N(nameof(RemainingPercentage));
                    }
                    EnragedChanged?.Invoke();
                    break;
                case nameof(NPC.RemainingEnrageTime):
                    if (!_serverSentEnrage && NPC.RemainingEnrageTime/1000 != 0)
                    {
                        NPC.EnragePattern.Duration = NPC.RemainingEnrageTime / 1000;
                        _serverSentEnrage = true;
                    }
                    if (CurrentEnrageTime < NPC.RemainingEnrageTime / 1000) ReEnraged?.Invoke();
                    _numberTimer.Refresh();
                    CurrentEnrageTime = NPC.RemainingEnrageTime / 1000;
                    N(nameof(EnrageTBtext));
                    N(nameof(RemainingPercentage));

                    break;

            }

        }
    }
}