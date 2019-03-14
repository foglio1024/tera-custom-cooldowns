using System;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public abstract class BaseClassLayoutVM : TSPropertyChanged, IDisposable
    {
        public virtual bool StartSpecialSkill(Cooldown sk)
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
            if (!Settings.SettingsHolder.ClassWindowSettings.Enabled) return;
            StaminaTracker.Max = v;
        }

        public void SetST(int currentStamina)
        {
            if (!Settings.SettingsHolder.ClassWindowSettings.Enabled) return;
            StaminaTracker.Val = currentStamina;
        }
        public BaseClassLayoutVM()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            StaminaTracker = new StatTracker();
        }

        public virtual void Dispose()
        {

        }
    }
}



