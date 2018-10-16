using System;
using System.Windows.Threading;
namespace TCC.Data
{
    public enum CooldownMode
    {
        Normal,
        Pre
    }
    public class FixedSkillCooldown : TSPropertyChanged
    {
        // events
        public event Action<CooldownMode> Started;
        public event Action<CooldownMode> Ended;
        public event Action FlashingForced;
        public event Action FlashingStopForced;
        public event Action SecondsUpdated;
        public event Action Reset;

        // fields
        private readonly DispatcherTimer _mainTimer;
        private readonly DispatcherTimer _offsetTimer;
        private readonly DispatcherTimer _secondsTimer;

        private CooldownType _cooldownType;
        private CooldownMode _currentMode;
        private ulong _seconds;
        private bool _flashOnAvailable;

        // properties
        public CooldownType CooldownType
        {
            get => _cooldownType;
            set
            {
                if (_cooldownType == value) return;
                _cooldownType = value;
                NPC();
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
                NPC();
            }
        }
        public ulong OriginalCooldown { get; private set; }
        public ulong Cooldown { get; private set; }
        public Skill Skill { get; }
        public bool FlashOnAvailable
        {
            get => _flashOnAvailable;
            set
            {
                _flashOnAvailable = value;
                NPC();
                if(value) ForceFlashing();
                else ForceStopFlashing();
            }
        }
        public bool IsAvailable => !_mainTimer.IsEnabled;

        // ctors
        public FixedSkillCooldown()
        {
            Dispatcher = App.BaseDispatcher;

            _mainTimer = new DispatcherTimer();
            _offsetTimer = new DispatcherTimer();
            _secondsTimer = new DispatcherTimer();

            _secondsTimer.Interval = TimeSpan.FromMilliseconds(1000);

            _mainTimer.Tick += CooldownEnded;
            _offsetTimer.Tick += StartSecondsTimer;
            _secondsTimer.Tick += DecreaseSeconds;

            SessionManager.CombatChanged += OnCombatStatusChanged;
            SessionManager.EncounterChanged += OnCombatStatusChanged;

        }

        private void OnCombatStatusChanged()
        {
            if ((SessionManager.Encounter || SessionManager.Combat) && FlashOnAvailable)
                ForceFlashing();
            else
                ForceStopFlashing();
        }

        public FixedSkillCooldown(Skill sk, bool flashOnAvailable, CooldownType t = CooldownType.Skill) : this()
        {
            CooldownType = t;
            Skill = sk;
            FlashOnAvailable = flashOnAvailable;
        }

        // timers tick handlers
        private void DecreaseSeconds(object sender, EventArgs e)
        {
            if (Seconds > 0) Seconds = Seconds - 1;
            else _secondsTimer.Stop();
        }
        private void StartSecondsTimer(object sender, EventArgs e)
        {
            _offsetTimer.Stop();
            _secondsTimer.Start();
        }
        private void CooldownEnded(object sender, EventArgs e)
        {
            _mainTimer.Stop();
            NPC(nameof(IsAvailable));
            _secondsTimer.Stop();
            Seconds = 0;
            Dispatcher.Invoke(() => Ended?.Invoke(_currentMode));
        }

        // methods
        private void ForceFlashing() => Dispatcher.Invoke(() => FlashingForced?.Invoke());

        public void Start(ulong cd, CooldownMode mode = CooldownMode.Normal)
        {
            if (cd >= Int32.MaxValue) return;
            if (_mainTimer.IsEnabled)
            {
                if (_currentMode == CooldownMode.Pre)
                {

                    _mainTimer.Stop();
                    NPC(nameof(IsAvailable));
                    _secondsTimer.Stop();
                    _offsetTimer.Stop();

                    Dispatcher.Invoke(() => Ended?.Invoke(_currentMode));
                }
            }

            _currentMode = mode;

            Seconds = cd / 1000;
            Cooldown = cd;
            OriginalCooldown = cd;

            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            _mainTimer.Start();
            NPC(nameof(IsAvailable));

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % 1000);
            _offsetTimer.Start();

            Dispatcher.Invoke(() => Started?.Invoke(_currentMode));
        }
        public void Refresh(ulong cd)
        {
            _mainTimer.Stop();
            NPC(nameof(IsAvailable));

            if (cd == 0 || cd >= Int32.MaxValue)
            {
                Seconds = 0;
                Cooldown = 0;
                Dispatcher?.Invoke(() => Ended?.Invoke(_currentMode));
                return;
            }

            Cooldown = cd;
            Seconds = Cooldown / 1000;

            //if (Seconds == 0)
            //{
            //    _secondsTimer.Stop();
            //    _dispatcher?.Invoke(() => Ended?.Invoke(_currentMode));
            //    return;
            //}

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % 1000);
            _offsetTimer.Start();

            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            _mainTimer.Start();
            NPC(nameof(IsAvailable));

            Dispatcher?.Invoke(() => Started?.Invoke(_currentMode));

        }
        public void Refresh(ulong id, ulong cd)
        {
            if (Skill.Id % 10 == 0 && id % 10 != 0) return; //TODO: check this; discards updates if new id is not base
            _mainTimer.Stop();
            NPC(nameof(IsAvailable));

            if (cd == 0 || cd >= Int32.MaxValue)
            {
                Seconds = 0;
                Cooldown = 0;
                Dispatcher?.Invoke(() => Ended?.Invoke(_currentMode));
                return;
            }

            Cooldown = cd;
            Seconds = Cooldown / 1000;

            _offsetTimer.Interval = TimeSpan.FromMilliseconds(cd % 1000);
            _offsetTimer.Start();

            _mainTimer.Interval = TimeSpan.FromMilliseconds(cd);
            _mainTimer.Start();
            NPC(nameof(IsAvailable));

            Dispatcher?.Invoke(() => Started?.Invoke(_currentMode));

        }

        public void ForceEnded() => CooldownEnded(null, null);
        public void ForceStopFlashing() => Dispatcher.Invoke(() => FlashingStopForced?.Invoke());
        public void ProcReset()
        {
            Dispatcher.Invoke(() => Reset?.Invoke());
        }
        public override string ToString()
        {
            return Skill.Name;
        }
    }
}
