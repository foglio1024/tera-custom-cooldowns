using System;
using System.Windows.Threading;
namespace TCC.Data
{
    public class FixedSkillCooldown : TSPropertyChanged
    {
        //TODO: remove this NPC thing and use proper events

        private readonly DispatcherTimer _secondsTimer;
        private readonly DispatcherTimer _offsetTimer;
        private readonly DispatcherTimer _shortTimer;
        public Skill Skill { get; }
        private ulong _cooldown;
        public ulong Cooldown
        {
            get => _cooldown;
            private set
            {
                if (_cooldown == value) return;
                _cooldown = value;
                NPC();
            }
        }
        public ulong OriginalCooldown { get; private set; }
        public ulong PreCooldown { get; private set; }
        private bool _isAvailable = true;
        public bool IsAvailable
        {
            get => _isAvailable;
            private set
            {
                if (_isAvailable == value) return;
                _isAvailable = value;
                NPC();
            }
        }

        private bool _isPreRunning;
        private CooldownType _cooldownType;

        public CooldownType CooldownType
        {
            get => _cooldownType;
            set
            {
                if(_cooldownType == value) return;
                _cooldownType = value;
                NPC();
            }
        }

        private ulong _seconds;
        private bool _flashOnAvailable;

        public ulong Seconds
        {
            get => _seconds;
            private set
            {
                if (_seconds == value) return;
                _seconds = value;
                NPC();
            }
        }

        public bool FlashOnAvailable
        {
            get => _flashOnAvailable;
            set
            {
                if (_flashOnAvailable == value) return;
                _flashOnAvailable = value;
                NPC(nameof(FlashOnAvailable));
                NPC(nameof(IsAvailable));
            }
        }
        public override string ToString()
        {
            return Skill.Name;
        }
        public FixedSkillCooldown(Skill sk, Dispatcher d, bool flashOnAvailable, CooldownType t = CooldownType.Skill)
        {
            _dispatcher = d;

            _cooldownType = t;
            _shortTimer = new DispatcherTimer(DispatcherPriority.Background, _dispatcher);
            _shortTimer.Tick += _shortTimer_Tick;

            Seconds = 0;

            _secondsTimer = new DispatcherTimer(DispatcherPriority.Background, _dispatcher);
            _secondsTimer.Interval = TimeSpan.FromSeconds(1);
            _secondsTimer.Tick += _secondsTimer_Tick;

            _offsetTimer = new DispatcherTimer(DispatcherPriority.Background, _dispatcher);
            _offsetTimer.Tick += _offsetTimer_Tick;

            Skill = sk;

            FlashOnAvailable = flashOnAvailable;
        }

        private void _shortTimer_Tick(object sender, EventArgs e)
        {
            _shortTimer.Stop();
            IsAvailable = true;
        }

        private void _offsetTimer_Tick(object sender, EventArgs e)
        {
            _offsetTimer.Stop();
            Seconds--;
            _secondsTimer.Start();

        }

        public FixedSkillCooldown(Dispatcher d)
        {
            _dispatcher = d;
        }
        private void _secondsTimer_Tick(object sender, EventArgs e)
        {
            var s = Seconds;
            if(Seconds == 1)
            {
                _secondsTimer.Stop();
                IsAvailable = true;
            }
            Seconds--;
            if (Seconds > s)
            {
                _secondsTimer.Stop();
                IsAvailable = true;
            }
        }

        public void Start(ulong cd)
        {
            if (_isPreRunning)
            {
                StopTimers();
                NPC("StopPre");
            }
            if (cd > 1000)
            {
                Cooldown = cd;//_type == CooldownType.Skill ? cd : cd * 1000;
                OriginalCooldown = Cooldown;
                Seconds = 1 + (Cooldown / 1000);
                var offset = Cooldown % 1000;
                _offsetTimer.Interval = TimeSpan.FromMilliseconds(offset);
                _offsetTimer.Start();
                IsAvailable = false;
            }
            else
            {
                Cooldown = cd;
                _shortTimer.Interval = TimeSpan.FromMilliseconds(cd);
                _shortTimer.Start();
                Seconds = 0;
                IsAvailable = false;
            }
            NPC("Start");
        }
        public void Refresh(ulong cd)
        {
            _secondsTimer.Stop();
            if (cd == 0)
            {
                IsAvailable = true;
                Cooldown = 0;
                Seconds = 0;
                NPC("Refresh");
                return;
            }
            Cooldown = cd;
            Seconds = Cooldown / 1000;
            if(Seconds == 0)
            {
                _secondsTimer.Stop();
                IsAvailable = true;
                return;
            }
            _offsetTimer.Interval = TimeSpan.FromMilliseconds(Cooldown % 1000);
            _offsetTimer.Start();
            NPC("Refresh");
        }
        public void ForceAvailable(bool available)
        {
            IsAvailable = available;
        }

        public void StartPre(ulong cd)
        {
            if (cd > 1000)
            {
                PreCooldown = cd;//_type == CooldownType.Skill ? cd : cd * 1000;
                OriginalCooldown = Cooldown;
                Seconds = 1 + (PreCooldown / 1000);
                var offset = PreCooldown % 1000;
                _offsetTimer.Interval = TimeSpan.FromMilliseconds(offset);
                _offsetTimer.Start();
                IsAvailable = false;
                _isPreRunning = true;
            }
            else
            {
                PreCooldown = cd;
                _shortTimer.Interval = TimeSpan.FromMilliseconds(cd);
                _shortTimer.Start();
                Seconds = 0;
                IsAvailable = false;
                _isPreRunning = true;
            }
            NPC("StartPre");

        }

        private void StopTimers()
        {
            _shortTimer.Stop();
            _offsetTimer.Stop();
            _secondsTimer.Stop();
        }
    }
}
