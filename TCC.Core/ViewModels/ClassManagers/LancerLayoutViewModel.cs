using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

internal class LancerLayoutViewModel : BaseClassLayoutViewModel
{
    public SkillWithEffect AdrenalineRush { get;  }
    public SkillWithEffect GuardianShout { get; }
    public Cooldown Infuriate { get; }
    public LancerLineHeldTracker LH { get; }
    public LancerLayoutViewModel()
    {
        LH = new LancerLineHeldTracker();
        Game.Me.Death += OnDeath;
        Game.DB!.SkillsDatabase.TryGetSkill(70300, Class.Lancer, out var gshout);
        GuardianShout = new SkillWithEffect(_dispatcher, gshout);

        Game.DB.SkillsDatabase.TryGetSkill(170200, Class.Lancer, out var arush);
        AdrenalineRush = new SkillWithEffect(_dispatcher, arush);

        Game.DB.SkillsDatabase.TryGetSkill(120100, Class.Lancer, out var infu);
        Infuriate = new Cooldown(infu, true) { CanFlash = true };

    }

    public override bool StartSpecialSkill(Cooldown sk)
    {
        if (sk.Skill.IconName == GuardianShout.Cooldown.Skill.IconName)
        {
            GuardianShout.StartCooldown(sk.Duration);
            return true;
        }
        if (sk.Skill.IconName == AdrenalineRush.Cooldown.Skill.IconName)
        {
            AdrenalineRush.StartCooldown(sk.Duration);
            return true;
        }

        if (sk.Skill.IconName != Infuriate.Skill.IconName) return false;
        Infuriate.Start(sk.Duration);
        return true;
    }

    void OnDeath()
    {
        LH.Stop();
        GuardianShout.StopEffect();
        AdrenalineRush.StopEffect();
    }

    public override void Dispose()
    {
        GuardianShout.Dispose();
        AdrenalineRush.Dispose();
        Infuriate.Dispose();
    }
}