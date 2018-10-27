using System;
using System.Windows.Threading;

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

        // properties
        public Skill Skill { get; }
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
                NPC();
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
                Dispatcher.Invoke(() => SecondsUpdated?.Invoke());
            }
        }
        public bool IsAvailable => !_mainTimer.IsEnabled;

        // ctors
        public Cooldown()
        {
            Dispatcher = App.BaseDispatcher;
            Dispatcher.Invoke(() =>
            {
                _mainTimer = new DispatcherTimer();
                _offsetTimer = new DispatcherTimer();
                _secondsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1)};
            });


            _mainTimer.Tick += CooldownEnded;
            _offsetTimer.Tick += StartSecondsTimer;
            _secondsTimer.Tick += DecreaseSeconds;

            SessionManager.CombatChanged += OnCombatStatusChanged;
            SessionManager.EncounterChanged += OnCombatStatusChanged;

        }
        public Cooldown(Skill sk, bool flashOnAvailable, CooldownType t = CooldownType.Skill) : this()
        {
            CooldownType = t;
            Skill = sk;
            FlashOnAvailable = flashOnAvailable;
        }

        public Cooldown(Skill sk, ulong cooldown,  CooldownType type = CooldownType.Skill, CooldownMode mode = CooldownMode.Normal) : this(sk, false, type)
        {
            if (cooldown == 0) return;

            Start(cooldown, mode);
        }
        private void OnCombatStatusChanged()
        {
            if ((SessionManager.Encounter || SessionManager.Combat) && FlashOnAvailable)
                ForceFlashing();
            else
                ForceStopFlashing();
        }

        // timers tick handlers

        private void CooldownEnded(object sender, EventArgs e)
        {
            _mainTimer.Stop();
            NPC(nameof(IsAvailable));
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
            if (cd >= Int32.MaxValue) return;
            if (_mainTimer.IsEnabled)
            {
                if (Mode == CooldownMode.Pre)
                {

                    _mainTimer.Stop();
                    NPC(nameof(IsAvailable));
                    _secondsTimer.Stop();
                    _offsetTimer.Stop();

                    Dispatcher.Invoke(() => Ended?.Invoke(Mode));
                }
            }

            Mode = mode;

            Seconds = cd / 1000;
            Duration = cd;
            OriginalDuration = cd;

            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            _mainTimer.Start();
            NPC(nameof(IsAvailable));

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % 1000);
            _offsetTimer.Start();

            Dispatcher.Invoke(() => Started?.Invoke(Mode));
        }
        public void Refresh(ulong cd)
        {
            _mainTimer.Stop();
            NPC(nameof(IsAvailable));

            if (cd == 0 || cd >= Int32.MaxValue)
            {
                Seconds = 0;
                Duration = 0;
                Dispatcher?.Invoke(() => Ended?.Invoke(Mode));
                return;
            }

            Duration = cd;
            Seconds = Duration / 1000;

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % 1000);
            _offsetTimer.Start();

            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            _mainTimer.Start();
            NPC(nameof(IsAvailable));

            Dispatcher?.Invoke(() => Started?.Invoke(Mode));

        }
        public void Refresh(ulong id, ulong cd)
        {
            if (Skill.Id % 10 == 0 && id % 10 != 0) return; //TODO: check this; discards updates if new id is not base
            Refresh(cd);
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
            _mainTimer.Stop(); //DispatcherTimer doesen't implement IDisposable
            _offsetTimer.Stop();
            _secondsTimer.Stop();
            SessionManager.CombatChanged -= OnCombatStatusChanged;
            SessionManager.EncounterChanged -= OnCombatStatusChanged;
        }
        public override string ToString()
        {
            return Skill.Name;
        }

    }
}
