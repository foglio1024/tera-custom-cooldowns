using System;
using System.Windows.Threading;
using FoglioUtils;

namespace TCC.Data.Skills
{
    public class Cooldown : TSPropertyChanged, IDisposable
    {
        // events
        public event Action<CooldownMode> Started;
        public event Action<CooldownMode> Ended;
        public event Action FlashingForced;
        public event Action FlashingStopForced;
        public event Action SecondsUpdated;
        public event Action Reset;

        // fields
        private DispatcherTimer _mainTimer;
        private DispatcherTimer _offsetTimer;
        private DispatcherTimer _secondsTimer;
        private ulong _seconds;
        private bool _flashOnAvailable;
        private bool _canFlash;
        private Skill _skill;

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
        public CooldownType CooldownType { get; set; }
        public CooldownMode Mode { get; private set; }
        public bool FlashOnAvailable
        {
            get => _flashOnAvailable;
            set
            {
                _flashOnAvailable = value;
                N();
                if (value) ForceFlashing();
                else ForceStopFlashing();
            }
        }
        public ulong Seconds
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
        public Cooldown(Dispatcher d = null)
        {
            Dispatcher = d ?? Dispatcher.CurrentDispatcher;
            Dispatcher.Invoke(() =>
            {
                _mainTimer = new DispatcherTimer();
                _offsetTimer = new DispatcherTimer();
                _secondsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            });


            _mainTimer.Tick += CooldownEnded;
            _offsetTimer.Tick += StartSecondsTimer;
            _secondsTimer.Tick += DecreaseSeconds;

        }
        public Cooldown(Skill sk, bool flashOnAvailable, CooldownType t = CooldownType.Skill, Dispatcher d = null) : this(d)
        {
            CooldownType = t;
            Skill = sk;
            FlashOnAvailable = flashOnAvailable;
        }
        public Cooldown(Skill sk, ulong cooldown, CooldownType type = CooldownType.Skill, CooldownMode mode = CooldownMode.Normal, Dispatcher d = null) : this(sk, false, type, d)
        {
            if (cooldown == 0) return;
            if (type == CooldownType.Item) cooldown = cooldown * 1000;
            Start(cooldown, mode);
        }

        private void OnCombatStatusChanged()
        {
            if ((Game.Encounter || Game.Combat) && FlashOnAvailable)
                ForceFlashing();
            else
                ForceStopFlashing();
        }

        // timers tick handlers

        private void CooldownEnded(object sender, EventArgs e)
        {
            _mainTimer.Stop();
            N(nameof(IsAvailable));
            _secondsTimer.Stop();
            Seconds = 0;
            Dispatcher.Invoke(() => Ended?.Invoke(Mode));
        }
        private void StartSecondsTimer(object sender, EventArgs e)
        {
            _offsetTimer.Stop();
            _secondsTimer.Start();
        }
        private void DecreaseSeconds(object sender, EventArgs e)
        {
            if (Seconds > 0) Seconds = Seconds - 1;
            else _secondsTimer.Stop();
        }

        // methods
        public void Start(ulong cd, CooldownMode mode = CooldownMode.Normal)
        {
            Duration = cd;
            OriginalDuration = cd;
            Seconds = Duration / 1000;
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
            Seconds = sk.Seconds;
            Duration = sk.Duration;
            OriginalDuration = sk.OriginalDuration;

            _mainTimer.Interval = TimeSpan.FromMilliseconds(Duration);
            _mainTimer.Start();
            N(nameof(IsAvailable));

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(Duration % 1000);
            _offsetTimer.Start();

            Dispatcher.Invoke(() => Started?.Invoke(Mode));
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
            Seconds = Duration / 1000;

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % 1000);
            _offsetTimer.Start();

            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            _mainTimer.Start();
            N(nameof(IsAvailable));

            Dispatcher?.Invoke(() => Started?.Invoke(Mode));

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
            CanFlash = false;
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
