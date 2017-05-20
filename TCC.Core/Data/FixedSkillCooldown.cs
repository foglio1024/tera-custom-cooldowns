using System.Windows.Threading;
namespace TCC.Data
{
    public class FixedSkillCooldown : TSPropertyChanged
    {
        CooldownType _type;
        public Skill Skill { get; set; }
        public uint Cooldown { get; set; }
        public uint OriginalCooldown { get; set; }

        public FixedSkillCooldown(Skill sk, CooldownType t, Dispatcher d)
        {
            _dispatcher = d;
            _type = t;
            Skill = sk;
        }
        public void Start(uint cd)
        {
            Cooldown = _type == CooldownType.Skill ? cd : cd * 1000;
            OriginalCooldown = Cooldown;
            NotifyPropertyChanged("Start");
        }
        public void Refresh(uint cd)
        {
            Cooldown = cd;
            NotifyPropertyChanged("Refresh");
        }

    }
}
