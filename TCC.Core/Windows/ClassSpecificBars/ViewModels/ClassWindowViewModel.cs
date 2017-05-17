using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class ClassWindowViewModel : BaseINPC
    {
        public int SkillCount
        {
            get { return Math.Max(WarriorBarManager.Instance.SecondarySkills.Count, WarriorBarManager.Instance.MainSkills.Count); }
        }

        public ClassWindowViewModel()
        {

        }
        //add alwaysOnTop logic
    }
}
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
