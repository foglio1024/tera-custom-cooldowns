using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public abstract class ClassManager : TSPropertyChanged
    {
        public virtual bool StartSpecialSkill(SkillCooldown sk)
        {
            return false;
        }

        public abstract void LoadSpecialSkills();

        public virtual bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            return false;
        }

        public virtual bool ResetSpecialSkill(Skill skill)
        {
            return false;
        }

        public StatTracker StaminaTracker { get; set; }


        public void SetMaxST(int v)
        {
            if (!Settings.ClassWindowSettings.Enabled) return;
            StaminaTracker.Max = v;
        }

        public void SetST(int currentStamina)
        {
            if (!Settings.ClassWindowSettings.Enabled) return;
            StaminaTracker.Val = currentStamina;
        }
        public ClassManager()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            StaminaTracker = new StatTracker();
        }

    }
}



