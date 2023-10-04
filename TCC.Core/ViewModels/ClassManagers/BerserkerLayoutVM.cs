using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

public class BerserkerLayoutVM : BaseClassLayoutVM
{
    bool _isUnleashOn;
    bool _isUnleashOff = true;

    public SkillWithEffect FieryRage { get; set; }
    public SkillWithEffect Bloodlust { get; set; }
    public SkillWithEffect Unleash { get; set; }

    public Cooldown Dexter { get; set; }
    public Cooldown Sinister { get; set; }
    public Cooldown Rampage { get; set; }
    public Cooldown BeastFury { get; set; }

    public StatTracker SinisterTracker { get; set; }
    public StatTracker DexterTracker { get; set; }
    public StatTracker RampageTracker { get; set; }

    public bool IsUnleashOn
    {
        get => _isUnleashOn;
        set
        {
            if (_isUnleashOn == value) return;
            _isUnleashOn = value;
            N();
        }
    }

    public bool IsUnleashOff
    {
        get => _isUnleashOff;
        set
        {
            if (_isUnleashOff == value) return;
            _isUnleashOff = value;
            N();
        }
    }

    public BerserkerLayoutVM()
    {
        SinisterTracker = new StatTracker();
        DexterTracker = new StatTracker();
        RampageTracker = new StatTracker();

        Game.DB!.SkillsDatabase.TryGetSkill(80600, Class.Berserker, out var fr);
        FieryRage = new SkillWithEffect(_dispatcher, fr);
        Game.DB.SkillsDatabase.TryGetSkill(210200, Class.Berserker, out var bl);
        Bloodlust = new SkillWithEffect(_dispatcher, bl);
        Game.DB.SkillsDatabase.TryGetSkill(330100, Class.Berserker, out var ul);
        Unleash = new SkillWithEffect(_dispatcher, ul);

        Game.DB.SkillsDatabase.TryGetSkill(340100, Class.Berserker, out var dx);
        Dexter = new Cooldown(dx, false);
        Game.DB.SkillsDatabase.TryGetSkill(350100, Class.Berserker, out var sx);
        Sinister = new Cooldown(sx, false);
        Game.DB.SkillsDatabase.TryGetSkill(360100, Class.Berserker, out var rp);
        Rampage = new Cooldown(rp, false);
        Game.DB.SkillsDatabase.TryGetSkill(370100, Class.Berserker, out var bf);
        BeastFury = new Cooldown(bf, false);

    }

    public override bool StartSpecialSkill(Cooldown sk)
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

        if (sk.Skill.IconName != BeastFury.Skill.IconName) return false;
        BeastFury.Start(sk.Duration);
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