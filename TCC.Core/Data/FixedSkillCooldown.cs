using System.Windows.Threading;
namespace TCC.Data
{
    public class FixedSkillCooldown : TSPropertyChanged
    {
        CooldownType _type;
        public Skill Skill { get; set; }
        public int Cooldown { get; set; }
        public int OriginalCooldown { get; set; }

        public FixedSkillCooldown(Skill sk, CooldownType t, Dispatcher d)
        {
            _dispatcher = d;
            _type = t;
            Skill = sk;
        }
        public void Start(int cd)
        {
            Cooldown = _type == CooldownType.Skill ? cd : cd * 1000;
            OriginalCooldown = Cooldown;
            NotifyPropertyChanged("Start");
        }
        public void Refresh(int cd)
        {
            Cooldown = cd;
            NotifyPropertyChanged("Refresh");
        }

    }
}
