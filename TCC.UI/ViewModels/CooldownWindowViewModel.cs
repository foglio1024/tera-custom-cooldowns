using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class CooldownWindowViewModel : BaseINPC
    {
        public ObservableCollection<SkillCooldown> ShortSkills { get => SkillManager.NormalSkillsQueue; }
        public ObservableCollection<SkillCooldown> LongSkills { get => SkillManager.LongSkillsQueue; }

        private void RemoveSkill(Skill sk, int cd)
        {
            if (cd < SkillManager.LongSkillTreshold)  //remove short skill
            {
                if (ShortSkills.Any(x => x.Skill == sk))
                {
                    ShortSkills.Remove(ShortSkills.FirstOrDefault(x => x.Skill == sk));
                }
            }
            else // remove long skill
            {
                if (LongSkills.Any(x => x.Skill == sk))
                {
                    LongSkills.Remove(LongSkills.FirstOrDefault(x => x.Skill == sk));
                }
            }
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }

        public CooldownWindowViewModel()
        {
            SkillIconControl.SkillEnded += RemoveSkill;
            WindowManager.TccVisibilityChanged += (s, ev) => RaisePropertyChanged("IsTeraOnTop");

        }
    }
}
