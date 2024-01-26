using System;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels.ClassManagers;

public abstract class BaseClassLayoutViewModel : ThreadSafeObservableObject, IDisposable
{
    public bool StartSpecialSkill(Cooldown cd)
    {
        var ret = StartSpecialSkillImpl(cd);
        cd.Dispose();
        return ret;
    }
    protected virtual bool StartSpecialSkillImpl(Cooldown cd)
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

    public StatTracker StaminaTracker { get; set; } = new();


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

    public virtual void Dispose()
    {

    }
}