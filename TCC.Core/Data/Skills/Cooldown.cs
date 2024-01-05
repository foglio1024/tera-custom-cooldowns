using Nostrum.WPF.ThreadSafe;
using System;
using System.Windows.Threading;
using TCC.Debugging;

namespace TCC.Data.Skills;

public class Cooldown : ThreadSafeObservableObject, IDisposable
{
    // events

    public event Action<ulong, CooldownMode>? Started;

    public event Action<CooldownMode>? Ended;

    public event Action? FlashingForced;

    public event Action? FlashingStopForced;

    public event Action? SecondsUpdated;

    public event Action? Reset;

    // fields

    readonly DispatcherTimer _mainTimer;
    readonly DispatcherTimer _offsetTimer;
    readonly DispatcherTimer _secondsTimer;
    double _seconds;
    bool _flashOnAvailable;
    bool _canFlash;
    Skill _skill;
    DateTime _endTime;
    CooldownMode _mode;
    bool _isAvailable;

    // properties

    public double Interval { get; }

    public Skill Skill
    {
        get => _skill;
        set
        {
            if (_skill == value) return;
            _skill = value;
            N();
        }
    }

    public ulong Duration { get; private set; }
    public ulong OriginalDuration { get; private set; }
    public CooldownType CooldownType { get; }

    public CooldownMode Mode
    {
        get => _mode; private set
        {
            if (_mode == value) return;
            _mode = value;
            N();
        }
    }

    public bool FlashOnAvailable
    {
        get => _flashOnAvailable && App.Settings.ClassWindowSettings.FlashAvailableSkills;
        set
        {
            _flashOnAvailable = value;
            N();
            if (value) ForceFlashing();
            else ForceStopFlashing();
        }
    }

    public double Seconds
    {
        get => _seconds;
        private set
        {
            if (_seconds == value) return;
            _seconds = value;
            N();
            _dispatcher.Invoke(() => SecondsUpdated?.Invoke());
        }
    }

    public bool IsAvailable
    {
        get => _isAvailable;
        private set => RaiseAndSetIfChanged(value, ref _isAvailable);
    }

    public bool CanFlash
    {
        get => _canFlash;
        set
        {
            if (_canFlash == value) return;
            _canFlash = value;
            if (value)
            {
                Game.CombatChanged += OnCombatStatusChanged;
                Game.EncounterChanged += OnCombatStatusChanged;
            }
            else
            {
                Game.CombatChanged -= OnCombatStatusChanged;
                Game.EncounterChanged -= OnCombatStatusChanged;
            }
        }
    }

    // ctors

    public Cooldown(Skill sk, bool flashOnAvailable, CooldownType t = CooldownType.Skill, Dispatcher? d = null, double intervalMs = 100) : base(d)
    {
        ObjectTracker.Register(GetType());
        Interval = intervalMs;
        _mainTimer = _dispatcher.Invoke(() => new DispatcherTimer());
        _offsetTimer = _dispatcher.Invoke(() => new DispatcherTimer());
        _secondsTimer = _dispatcher.Invoke(() => new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Interval) });

        _mainTimer.Tick += CooldownEnded;
        _offsetTimer.Tick += StartSecondsTimer;
        _secondsTimer.Tick += DecreaseSeconds;

        App.Settings.ClassWindowSettings.FlashAvailableSkillsChanged += OnGlobalFlashChanged;

        _skill = sk;
        CooldownType = t;
        FlashOnAvailable = flashOnAvailable;
    }

    public Cooldown(Skill sk, ulong cooldown, CooldownType type = CooldownType.Skill, CooldownMode mode = CooldownMode.Normal, Dispatcher? d = null, double intervalMs = 100) : this(sk, false, type, d, intervalMs)
    {
        if (cooldown == 0) return;
        if (type == CooldownType.Item) cooldown *= 1000;
        Start(cooldown, mode);
    }

    ~Cooldown()
    {
        ObjectTracker.Unregister(GetType());
    }

    void OnGlobalFlashChanged()
    {
        _dispatcher.InvokeAsync(OnCombatStatusChanged);
    }

    void OnCombatStatusChanged()
    {
        _dispatcher.InvokeAsync(() =>
        {
            if ((Game.Encounter || Game.Combat) && FlashOnAvailable)
            {
                ForceFlashing();
            }
            else
            {
                ForceStopFlashing();
            }
        });
    }

    // timers tick handlers

    void CooldownEnded(object? sender, EventArgs? e)
    {
        StopMainTimer();
        _secondsTimer.Stop();
        Seconds = 0;
        _dispatcher.Invoke(() => Ended?.Invoke(Mode));
    }

    void StartSecondsTimer(object? sender, EventArgs? e)
    {
        _offsetTimer.Stop();
        _secondsTimer.Start();
    }

    void DecreaseSeconds(object? sender, EventArgs? e)
    {
        if (Seconds > 0)
        {
            Seconds = (_endTime - DateTime.Now).TotalMilliseconds / 1000D;
        }
        else
        {
            _secondsTimer.Stop();
        }
    }

    // methods

    public void Start(ulong cd, CooldownMode mode = CooldownMode.Normal)
    {
        Duration = cd;
        OriginalDuration = cd;
        var now = DateTime.Now;
        _endTime = now.AddMilliseconds(Duration);
        Seconds = (_endTime - now).TotalMilliseconds / 1000D;
        Mode = mode;
        Start(this);
    }

    public void Start(Cooldown sk)
    {
        if (sk != this)
        {
            sk.Dispose();
        }

        if (sk.Duration >= int.MaxValue) return;

        if (_mainTimer.IsEnabled && Mode is CooldownMode.Pre)
        {
            StopMainTimer();
            _secondsTimer.Stop();
            _offsetTimer.Stop();
            _dispatcher.Invoke(() => Ended?.Invoke(Mode));
        }

        Mode = sk.Mode;
        Duration = sk.Duration;
        OriginalDuration = sk.OriginalDuration;
        var now = DateTime.Now;
        _endTime = now.AddMilliseconds(Duration);
        Seconds = (_endTime - now).TotalMilliseconds / 1000D;
        _mainTimer.Interval = TimeSpan.FromMilliseconds(Duration);
        StartMainTimer();

        _offsetTimer.Interval = TimeSpan.FromMilliseconds(Duration % Interval);
        _offsetTimer.Start();
        _dispatcher.Invoke(() => Started?.Invoke(Duration, Mode));
    }

    public void Stop()
    {
        StopMainTimer();
        Seconds = 0;
        Duration = 0;
        _dispatcher.Invoke(() => Ended?.Invoke(Mode));
    }

    public void Refresh(ulong cd, CooldownMode mode)
    {
        StopMainTimer();

        if (cd is 0 or >= int.MaxValue)
        {
            Seconds = 0;
            Duration = 0;
            _dispatcher.Invoke(() => Ended?.Invoke(Mode));
        }
        else
        {
            Mode = mode;
            Duration = cd;
            var now = DateTime.Now;
            _endTime = now.AddMilliseconds(Duration);
            Seconds = (_endTime - now).TotalMilliseconds / 1000D;

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % Interval);
            _offsetTimer.Start();
            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            StartMainTimer();
            _dispatcher.Invoke(() => Started?.Invoke(Duration, Mode));
        }
    }

    public void Refresh(ulong id, ulong cd, CooldownMode mode)
    {
        if (Skill.Id % 10 == 0 && id % 10 != 0) return; //TODO: check this; discards updates if new id is not base
        Refresh(cd, mode);
    }

    public void Refresh(Cooldown cd)
    {
        cd.Dispose();
        Refresh(cd.Skill.Id, cd.Duration, cd.Mode);
    }

    void ForceFlashing()
    {
        _dispatcher.InvokeAsync(() => FlashingForced?.Invoke());
    }

    void StartMainTimer()
    {
        IsAvailable = false;
        _mainTimer.Start();
    }

    void StopMainTimer()
    {
        _mainTimer.Stop();
        IsAvailable = true;
    }

    public void ForceStopFlashing()
    {
        _dispatcher.InvokeAsync(() => FlashingStopForced?.Invoke());
    }

    public void ForceEnded()
    {
        CooldownEnded(null, null);
    }

    public void ProcReset()
    {
        _dispatcher.Invoke(() => Reset?.Invoke());
    }

    public void Dispose()
    {
        App.Settings.ClassWindowSettings.FlashAvailableSkillsChanged -= OnGlobalFlashChanged;
        CanFlash = false;

        Game.CombatChanged -= OnCombatStatusChanged;
        Game.EncounterChanged -= OnCombatStatusChanged;

        _mainTimer.Tick -= CooldownEnded;
        _offsetTimer.Tick -= StartSecondsTimer;
        _secondsTimer.Tick -= DecreaseSeconds;

        _mainTimer.Stop();
        _offsetTimer.Stop();
        _secondsTimer.Stop();
    }

    public override string ToString()
    {
        return Skill.Name;
    }
}