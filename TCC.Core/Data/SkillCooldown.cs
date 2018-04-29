using System;
using System.Timers;
using System.Windows.Threading;

namespace TCC
{
    public class SkillCooldown : TSPropertyChanged, IDisposable
    {
        public Dispatcher Dispatcher { get => _dispatcher; }
        public Skill Skill { get; set; }
        public ulong Cooldown { get; set; }
        public ulong OriginalCooldown { get; set; }
        public CooldownType Type { get; set; }
        private Timer _timer;

        public SkillCooldown(Skill sk, ulong cd, CooldownType t, Dispatcher d)
        {
            _dispatcher = d;

            var cooldown = cd > int.MaxValue ? int.MaxValue : cd;

            Skill = sk;
            Cooldown = t==CooldownType.Skill ? cooldown : cooldown * 1000;
            Type = t;
            OriginalCooldown = Cooldown;

            if (cooldown == 0) return;
            _timer = new Timer(Cooldown);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NPC("Ending");
            _timer?.Stop();
        }

        public void Refresh(ulong cd)
        {
            Cooldown = cd;
            NPC("Refresh");
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