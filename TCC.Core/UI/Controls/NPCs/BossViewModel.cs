using Nostrum;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Npc;

namespace TCC.UI.Controls.NPCs;

public class BossViewModel : NpcViewModel
{
    private double _nextEnragePerc;
    private bool _isTimerRunning;
    private bool _serverSentEnrage;

    private readonly DispatcherTimer _numberTimer;

    public event Action? EnragedChanged;
    public event Action? ReEnraged;

    public ObservableCollection<EnragePeriodItem> EnrageHistory { get; set; }
    public string MainPercInt => ShowHP ? Convert.ToInt32(Math.Floor(NPC.HPFactor * 100)).ToString() : "?";
    public string MainPercDec
    {
        get
        {
            if (!ShowHP) return "??";
            var val = NPC.HPFactor * 100 % 1 * 100;
            val = val > 99 ? 99 : val;
            return $"{val:00}";

        }
    }
    public double TotalEnrage
    {
        get
        {
            if (EnrageHistory.Count == 0) return 0;
            return EnrageHistory.Sum(enragePeriodItem => enragePeriodItem.Duration);
        }
    }
    public double CurrentPercentage => NPC.HPFactor * 100;
    public double RemainingPercentage
    {
        get
        {
            var percentage = NPC.EnragePattern?.Percentage;
            if (percentage == null) return 0;
            return (CurrentPercentage - NextEnragePercentage) / percentage.Value > 0
                ? (CurrentPercentage - NextEnragePercentage) / percentage.Value
                : 0;
        }
    }

    public double NextEnragePercentage
    {
        get => _nextEnragePerc;
        private set
        {
            if (_nextEnragePerc == value) return;
            _nextEnragePerc = value;
            if (value < 0) _nextEnragePerc = 0;
            InvokePropertyChanged();
            InvokePropertyChanged(nameof(EnrageTBtext));
        }
    }
    public string EnrageTBtext
    {
        get
        {
            if (NPC.Enraged)
            {
                return NPC.EnragePattern?.StaysEnraged == true ? "∞" : $"{TimeUtils.FormatSeconds(CurrentEnrageTime)}";
            }

            return App.Settings.NpcWindowSettings.EnrageLabelMode switch
            {
                EnrageLabelMode.Next => $"{NextEnragePercentage:0.#}%",
                EnrageLabelMode.Remaining => $"{CurrentPercentage - NextEnragePercentage:0.#}%",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    public string RhombEnrageTimerText
    {
        get
        {
            return NPC.Enraged
                ? NPC.EnragePattern?.StaysEnraged == true
                    ? "∞"
                    : NPC.EnragePattern?.Duration != 0
                        ? $"{TimeUtils.FormatSeconds(CurrentEnrageTime)}"
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
            if (!RaiseAndSetIfChanged(value, ref _curEnrageTime)) return;
            InvokePropertyChanged(nameof(EnrageTBtext));
            InvokePropertyChanged(nameof(RhombEnrageTimerText));
        }
    }

    public bool IsTimerRunning
    {
        get => _isTimerRunning;
        set => RaiseAndSetIfChanged(value, ref _isTimerRunning);
    }

    public BossViewModel(Npc npc) : base(npc)
    {

        _numberTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _numberTimer.Tick += OnNumberTimerTick;


        EnrageHistory = new ThreadSafeObservableCollection<EnragePeriodItem>();

        if (NPC.EnragePattern != null)
        {
            NextEnragePercentage = 100 - NPC.EnragePattern.Percentage;
            CurrentEnrageTime = NPC.EnragePattern.StaysEnraged ? int.MaxValue : NPC.EnragePattern.Duration;
        }


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
        if (NPC.EnragePattern?.StaysEnraged == false) CurrentEnrageTime--;
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

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(NPC.HPFactor):
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
                        //InvokePropertyChanged(nameof(EnrageHistory));
                    }
                }
                InvokeHpChanged();
                InvokePropertyChanged(nameof(EnrageTBtext));
                InvokePropertyChanged(nameof(RemainingPercentage));
                InvokePropertyChanged(nameof(TotalEnrage));
                InvokePropertyChanged(nameof(MainPercDec));
                InvokePropertyChanged(nameof(MainPercInt));
                break;
            case nameof(NPC.MaxHP):
                if (NPC is { HPFactor: 1, EnragePattern: not null })
                {
                    NextEnragePercentage = 100 - NPC.EnragePattern.Percentage;
                }
                break;
            case nameof(NPC.Enraged):
                if (NPC.Enraged)
                {
                    EnrageHistory.Add(new EnragePeriodItem(CurrentPercentage));
                    _addEnrageItem = false;
                    _numberTimer.Refresh();
                    InvokePropertyChanged(nameof(RhombEnrageTimerText));
                    //InvokePropertyChanged(nameof(EnrageHistory));
                }
                else
                {
                    _addEnrageItem = true;
                    _serverSentEnrage = false;
                    _numberTimer.Stop();
                    if (NPC.EnragePattern != null)
                    {
                        NextEnragePercentage = CurrentPercentage - NPC.EnragePattern.Percentage;
                        CurrentEnrageTime = NPC.EnragePattern.StaysEnraged
                            ? int.MaxValue
                            : NPC.EnragePattern.Duration;
                    }

                    InvokePropertyChanged(nameof(RemainingPercentage));
                }
                EnragedChanged?.Invoke();
                break;
            case nameof(NPC.RemainingEnrageTime):
                if (!_serverSentEnrage && NPC.RemainingEnrageTime / 1000 != 0)
                {
                    if (NPC.EnragePattern != null) NPC.EnragePattern.Duration = NPC.RemainingEnrageTime / 1000;
                    _serverSentEnrage = true;
                }
                if (CurrentEnrageTime < NPC.RemainingEnrageTime / 1000) ReEnraged?.Invoke();
                _numberTimer.Refresh();
                CurrentEnrageTime = NPC.RemainingEnrageTime / 1000;
                InvokePropertyChanged(nameof(EnrageTBtext));
                InvokePropertyChanged(nameof(RemainingPercentage));

                break;

        }

    }
}