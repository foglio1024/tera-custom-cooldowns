using System;
using System.Timers;
using System.Windows.Threading;
using TCC.ViewModels;

namespace TCC
{
    public class SkillCooldown : TSPropertyChanged, IDisposable
    {
        public Skill Skill { get; set; }
        public int Cooldown { get; set; }
        public int OriginalCooldown { get; set; }
        public CooldownType Type { get; set; }
        private Timer _timer;

        public SkillCooldown(Skill sk, int cd, CooldownType t)
        {
            _dispatcher = CooldownBarWindowManager.Instance.Dispatcher;


            Skill = sk;
            Cooldown = cd;
            OriginalCooldown = Cooldown;
            if (t == CooldownType.Item)
            {
                Cooldown = Cooldown * 1000;
            }

            if (cd == 0) return;
            _timer = new Timer(cd);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NotifyPropertyChanged("Ending");
            _timer?.Stop();
        }

        public void Refresh()
        {
            NotifyPropertyChanged("Refresh");
            if (_timer == null) return;
            _timer.Stop();
            _timer.Interval = Cooldown > 0 ? Cooldown : 1;
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Stop();
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