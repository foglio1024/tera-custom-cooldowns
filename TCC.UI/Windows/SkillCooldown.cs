using System.Windows.Threading;
using TCC.ViewModels;

namespace TCC
{
    public class SkillCooldown : TSPropertyChanged
    {
        public Skill Skill { get; set; }
        public int Cooldown { get; set; }
        public int OriginalCooldown { get; set; }
        public CooldownType Type { get; set; }

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
        }
        public void Refresh()
        {
            NotifyPropertyChanged("Refresh");
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/