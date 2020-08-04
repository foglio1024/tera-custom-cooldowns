using System;
using System.Windows.Threading;
using Nostrum;

namespace TCC.Data.Skills
{
    public class Cooldown : TSPropertyChanged, IDisposable
    {
        // events
        public event Action<ulong, CooldownMode> Started = null!;
        public event Action<CooldownMode> Ended = null!;
        public event Action FlashingForced = null!;
        public event Action FlashingStopForced = null!;
        public event Action SecondsUpdated = null!;
        public event Action Reset = null!;

        // fields
        private readonly DispatcherTimer _mainTimer;
        private readonly DispatcherTimer _offsetTimer;
        private readonly DispatcherTimer _secondsTimer;
        private double _seconds;
        private bool _flashOnAvailable;
        private bool _canFlash;
        private Skill _skill;
        private DateTime _endTime;
        public double Interval { get; private set; }

        // properties
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
        public CooldownMode Mode { get; private set; }
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
            set
            {
                if (_seconds == value) return;
                _seconds = value;
                N();
                Dispatcher.Invoke(() => SecondsUpdated?.Invoke());
            }
        }
        public bool IsAvailable => !_mainTimer.IsEnabled;
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
        public Cooldown(Skill sk, bool flashOnAvailable, CooldownType t = CooldownType.Skill, Dispatcher? d = null, double intervalMs = 100)
        {
            Interval = intervalMs;
            Dispatcher = d ?? Dispatcher.CurrentDispatcher;
            _mainTimer = Dispatcher.Invoke(() => new DispatcherTimer());
            _offsetTimer = Dispatcher.Invoke(() => new DispatcherTimer());
            _secondsTimer = Dispatcher.Invoke(() => new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Interval) });

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
        private void OnGlobalFlashChanged()
        {
            Dispatcher.InvokeAsync(OnCombatStatusChanged);
        }

        private void OnCombatStatusChanged()
        {
            if ((Game.Encounter || Game.Combat) && FlashOnAvailable)
                ForceFlashing();
            else
                ForceStopFlashing();
        }

        // timers tick handlers

        private void CooldownEnded(object? sender, EventArgs? e)
        {
            _mainTimer.Stop();
            N(nameof(IsAvailable));
            _secondsTimer.Stop();
            Seconds = 0;
            Dispatcher.Invoke(() => Ended?.Invoke(Mode));
        }
        private void StartSecondsTimer(object? sender, EventArgs? e)
        {
            _offsetTimer.Stop();
            _secondsTimer.Start();
        }
        private void DecreaseSeconds(object? sender, EventArgs? e)
        {
            if (Seconds > 0)
            {
                var now = DateTime.Now;

                //Seconds -= Interval;
                Seconds = (_endTime - now).TotalMilliseconds/ 1000D;
            }
            else _secondsTimer.Stop();
        }

        // methods
        public void Start(ulong cd, CooldownMode mode = CooldownMode.Normal)
        {
            Duration = cd;
            OriginalDuration = cd;
            //Seconds = Duration - (Duration % Interval);
            var now = DateTime.Now;
            _endTime = now.AddMilliseconds(Duration);
            Seconds = (_endTime - now).TotalMilliseconds  / 1000D;

            Mode = mode;
            Start(this);
        }
        public void Start(Cooldown sk)
        {
            if (sk != this) sk.Dispose();
            if (sk.Duration >= int.MaxValue) return;
            if (_mainTimer.IsEnabled)
            {
                if (Mode == CooldownMode.Pre)
                {

                    _mainTimer.Stop();
                    N(nameof(IsAvailable));
                    _secondsTimer.Stop();
                    _offsetTimer.Stop();

                    Dispatcher.Invoke(() => Ended?.Invoke(Mode));
                }
            }

            Mode = sk.Mode;
            Duration = sk.Duration;
            var now = DateTime.Now;
            //Seconds = sk.Seconds - (Duration % Interval);
            _endTime = now.AddMilliseconds(Duration);
            Seconds = (_endTime - now).TotalMilliseconds / 1000D;

            OriginalDuration = sk.OriginalDuration;

            _mainTimer.Interval = TimeSpan.FromMilliseconds(Duration);
            _mainTimer.Start();
            N(nameof(IsAvailable));

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(Duration % Interval);
            _offsetTimer.Start();

            Dispatcher.Invoke(() => Started?.Invoke(Duration, Mode));
        }

        public void Stop()
        {
            _mainTimer.Stop();
            N(nameof(IsAvailable));
            Seconds = 0;
            Duration = 0;
            Dispatcher?.Invoke(() => Ended?.Invoke(Mode));
        }
        public void Refresh(ulong cd, CooldownMode mode)
        {
            _mainTimer.Stop();
            N(nameof(IsAvailable));

            if (cd == 0 || cd >= int.MaxValue)
            {
                Seconds = 0;
                Duration = 0;
                Dispatcher?.Invoke(() => Ended?.Invoke(Mode));
                return;
            }
            Mode = mode;
            Duration = cd;
            //Seconds = Duration / Interval;
            var now = DateTime.Now;
            _endTime = now.AddMilliseconds(Duration);
            Seconds = (_endTime - now).TotalMilliseconds / 1000D;

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % Interval);
            _offsetTimer.Start();

            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            _mainTimer.Start();
            N(nameof(IsAvailable));

            Dispatcher?.Invoke(() => Started?.Invoke(Duration, Mode));

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
        private void ForceFlashing()
        {
            Dispatcher.Invoke(() => FlashingForced?.Invoke());
        }
        public void ForceStopFlashing()
        {
            Dispatcher.Invoke(() => FlashingStopForced?.Invoke());
        }
        public void ForceEnded()
        {
            CooldownEnded(null, null);
        }
        public void ProcReset()
        {
            Dispatcher.Invoke(() => Reset?.Invoke());
        }
        public void Dispose()
        {
            App.Settings.ClassWindowSettings.FlashAvailableSkillsChanged -= OnGlobalFlashChanged;
            CanFlash = false;

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
}
