using TCC.Data;

namespace TCC.ViewModels
{
    public class SlayerBarManager : ClassManager
    {

        public DurationCooldownIndicator InColdBlood { get; set; }
        
        public FixedSkillCooldown OverhandStrike { get; set; }
        


        public override void LoadSpecialSkills()
        {
            // In Cold Blood
            SessionManager.SkillsDatabase.TryGetSkill(200200, Class.Slayer, out var icb);
            InColdBlood = new DurationCooldownIndicator(Dispatcher) {
                Buff = new FixedSkillCooldown(icb, false),
                Cooldown = new FixedSkillCooldown(icb, true)
            };

            // Overhand Strike
            SessionManager.SkillsDatabase.TryGetSkill(80900, Class.Slayer, out var ohs);
            OverhandStrike = new FixedSkillCooldown(ohs, false);

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
