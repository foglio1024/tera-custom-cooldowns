using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

public class SlayerLayoutViewModel : BaseClassLayoutViewModel
{

    public SkillWithEffect InColdBlood { get; }

    public Cooldown OverhandStrike { get; }



    public SlayerLayoutViewModel()
    {
        // In Cold Blood
        Game.DB!.SkillsDatabase.TryGetSkill(200200, Class.Slayer, out var icb);
        InColdBlood = new SkillWithEffect(_dispatcher, icb);

        // Overhand Strike
        Game.DB.SkillsDatabase.TryGetSkill(80900, Class.Slayer, out var ohs);
        OverhandStrike = new Cooldown(ohs, false);

    }

    public override void Dispose()
    {
        InColdBlood.Dispose();
    }

    public override bool StartSpecialSkill(Cooldown sk)
    {
        if (sk.Skill.IconName == InColdBlood.Cooldown.Skill.IconName)
        {
            InColdBlood.StartCooldown(sk.Duration);
            return true;
        }

        if (sk.Skill.IconName != OverhandStrike.Skill.IconName) return false;
        OverhandStrike.Start(sk.Duration);
        return true;

    }
}