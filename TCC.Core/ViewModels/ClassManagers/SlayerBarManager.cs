using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class SlayerBarManager : ClassManager
    {

        public DurationCooldownIndicator InColdBlood { get; set; }
        public FixedSkillCooldown OverhandStrike { get; set; }
        public SlayerBarManager() : base()
        {
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(200200, Class.Slayer, out var icb);
            SessionManager.SkillsDatabase.TryGetSkill(80900, Class.Slayer, out var ohs);

            InColdBlood =
                new DurationCooldownIndicator(_dispatcher)
                {
                    Buff = new FixedSkillCooldown(icb, false),
                    Cooldown = new FixedSkillCooldown(icb, true)
                };

            OverhandStrike =
                new FixedSkillCooldown(ohs, false);

        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == InColdBlood.Cooldown.Skill.IconName)
            {
                InColdBlood.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == OverhandStrike.Skill.IconName)
            {
                OverhandStrike.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
