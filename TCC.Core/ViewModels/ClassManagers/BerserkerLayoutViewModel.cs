using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

public class BerserkerLayoutViewModel : BaseClassLayoutViewModel
{
    bool _isUnleashOn;

    public SkillWithEffect FieryRage { get; set; }
    public SkillWithEffect Bloodlust { get; set; }
    public SkillWithEffect Unleash { get; set; }

    public SkillWithEffect Dexter { get; set; }
    public SkillWithEffect Sinister { get; set; }
    public SkillWithEffect Rampage { get; set; }
    public SkillWithEffect BeastFury { get; set; }

    public StatTracker DexterSinixterTracker { get; set; }
    public StatTracker RampageTracker { get; set; }

    public bool IsUnleashOn
    {
        get => _isUnleashOn;
        set => RaiseAndSetIfChanged(value, ref _isUnleashOn);
    }

    public BerserkerLayoutViewModel()
    {
        DexterSinixterTracker = new StatTracker { Max = 10, Val = 0 };
        RampageTracker = new StatTracker { Max = 10, Val = 0 };

        Game.DB!.SkillsDatabase.TryGetSkill(80600, Class.Berserker, out var fr);
        FieryRage = new SkillWithEffect(_dispatcher, fr);
        Game.DB.SkillsDatabase.TryGetSkill(210200, Class.Berserker, out var bl);
        Bloodlust = new SkillWithEffect(_dispatcher, bl);
        Game.DB.SkillsDatabase.TryGetSkill(330100, Class.Berserker, out var ul);
        Unleash = new SkillWithEffect(_dispatcher, ul);

        Game.DB.SkillsDatabase.TryGetSkill(340100, Class.Berserker, out var dx);
        Dexter = new SkillWithEffect(_dispatcher, dx);
        Game.DB.SkillsDatabase.TryGetSkill(350100, Class.Berserker, out var sx);
        Sinister = new SkillWithEffect(_dispatcher, sx);
        Game.DB.SkillsDatabase.TryGetSkill(360100, Class.Berserker, out var rp);
        Rampage = new SkillWithEffect(_dispatcher, rp);
        Game.DB.SkillsDatabase.TryGetSkill(370100, Class.Berserker, out var bf);
        BeastFury = new SkillWithEffect(_dispatcher, bf);

    }

    protected override bool StartSpecialSkillImpl(Cooldown sk)
    {
        if (sk.Skill.IconName == FieryRage.Cooldown.Skill.IconName)
        {
            FieryRage.StartCooldown(sk.Duration);
            return true;
        }
        if (sk.Skill.IconName == Bloodlust.Cooldown.Skill.IconName)
        {
            Bloodlust.StartCooldown(sk.Duration);
            return true;
        }
        if (sk.Skill.IconName == Unleash.Cooldown.Skill.IconName)
        {
            Unleash.StartCooldown(sk.Duration);
            return true;
        }

        if (sk.Skill.IconName != BeastFury.Cooldown.Skill.IconName) return false;
        BeastFury.Cooldown.Start(sk.Duration);
        return true;
    }

    public override bool ChangeSpecialSkill(Skill skill, uint cd)
    {
        if (skill.IconName != Unleash.Cooldown.Skill.IconName) return false;
        Unleash.RefreshCooldown(cd, skill.Id);
        //Unleash.Cooldown.Refresh(skill.Id, cd, CooldownMode.Normal);
        return true;
    }

    public override void Dispose()
    {
        FieryRage.Dispose();
        Bloodlust.Dispose();
        Unleash.Dispose();
    }
}