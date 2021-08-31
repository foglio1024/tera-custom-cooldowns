using System;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels.ClassManagers
{
    public abstract class BaseClassLayoutVM : ThreadSafePropertyChanged, IDisposable
    {
        public virtual bool StartSpecialSkill(Cooldown sk)
        {
            return false;
        }

        //public abstract void LoadSpecialSkills();

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
            if (!App.Settings.ClassWindowSettings.Enabled) return;
            StaminaTracker.Max = v;
        }

        public void SetST(int currentStamina)
        {
            if (!App.Settings.ClassWindowSettings.Enabled) return;
            StaminaTracker.Val = currentStamina;
        }
        public BaseClassLayoutVM()
        {
            SetDispatcher(Dispatcher.CurrentDispatcher);
            StaminaTracker = new StatTracker();
        }

        public virtual void Dispose()
        {

        }
    }
}



