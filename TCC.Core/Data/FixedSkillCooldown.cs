using System;
using System.Windows.Threading;
namespace TCC.Data
{
    public class FixedSkillCooldown : TSPropertyChanged
    {
        CooldownType _type;
        DispatcherTimer _secondsTimer;
        public Skill Skill { get; set; }
        public uint Cooldown { get; set; }
        public uint OriginalCooldown { get; set; }
        public bool IsAvailable { get; private set; }
        uint _secondsCooldown = 0;
        public uint Seconds
        {
            get
            {
                return _secondsCooldown;
            }
            set
            {
                if (_secondsCooldown == value) return;
                _secondsCooldown = value;
                NotifyPropertyChanged("Seconds");
            }
        }
        public FixedSkillCooldown(Skill sk, CooldownType t, Dispatcher d)
        {
            _dispatcher = d;
            Seconds = 0;
            _secondsTimer = new DispatcherTimer();
            _secondsTimer.Interval = TimeSpan.FromSeconds(1);
            _secondsTimer.Tick += _secondsTimer_Tick;
            _type = t;
            Skill = sk;
        }
        public FixedSkillCooldown(Dispatcher d)
        {
            _dispatcher = d;
        }
        private void _secondsTimer_Tick(object sender, EventArgs e)
        {
            if (Seconds == 0)
            {
                _secondsTimer.Stop();
                IsAvailable = true;
                NotifyPropertyChanged("IsAvailable");
                return;
            }
            Seconds--;
        }

        public void Start(uint cd)
        {
            Cooldown = _type == CooldownType.Skill ? cd : cd * 1000;
            OriginalCooldown = Cooldown;
            Seconds = Cooldown / 1000;
            _secondsTimer.Start();
            IsAvailable = false;
            NotifyPropertyChanged("Start");
            NotifyPropertyChanged("IsAvailable");

        }
        public void Refresh(uint cd)
        {
            _secondsTimer.Stop();
            Cooldown = cd;
            Seconds = Cooldown / 1000;
            _secondsTimer.Start();
            NotifyPropertyChanged("Refresh");
        }
    }
}
