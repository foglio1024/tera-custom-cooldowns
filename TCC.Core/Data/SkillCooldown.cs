using System;
using System.Timers;
using System.Windows.Threading;

namespace TCC
{
    public class SkillCooldown : TSPropertyChanged, IDisposable
    {
        public Dispatcher Dispatcher { get => _dispatcher; }
        public Skill Skill { get; set; }
        public uint Cooldown { get; set; }
        public uint OriginalCooldown { get; set; }
        private Timer _timer;

        public void SetDispatcher(Dispatcher d)
        {
            _dispatcher = d;
        }

        public SkillCooldown(Skill sk, uint cd, CooldownType t, Dispatcher d)
        {
            _dispatcher = d;


            Skill = sk;
            Cooldown = t==CooldownType.Skill ? cd : cd*1000;
            OriginalCooldown = Cooldown;

            if (cd == 0) return;
            _timer = new Timer(Cooldown);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NotifyPropertyChanged("Ending");
            _timer?.Stop();
        }

        public void Refresh(uint cd)
        {
            Cooldown = cd;
            NotifyPropertyChanged("Refresh");
            if (_timer == null) return;
            _timer.Stop();
            _timer.Interval = Cooldown > 0 ? Cooldown : 1;
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Stop();
            if(_timer != null) _timer.Elapsed -= _timer_Elapsed;
            _timer?.Dispose();
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/