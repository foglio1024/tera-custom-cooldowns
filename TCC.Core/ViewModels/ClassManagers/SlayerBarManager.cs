using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public class SlayerBarManager : ClassManager
    {

        public DurationCooldownIndicator InColdBlood { get; set; }
        
        public Cooldown OverhandStrike { get; set; }
        


        public override void LoadSpecialSkills()
        {
            // In Cold Blood
            SessionManager.SkillsDatabase.TryGetSkill(200200, Class.Slayer, out var icb);
            InColdBlood = new DurationCooldownIndicator(Dispatcher) {
                Buff = new Cooldown(icb, false),
                Cooldown = new Cooldown(icb, true)
            };

            // Overhand Strike
            SessionManager.SkillsDatabase.TryGetSkill(80900, Class.Slayer, out var ohs);
            OverhandStrike = new Cooldown(ohs, false);

        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == InColdBlood.Cooldown.Skill.IconName)
            {
                InColdBlood.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == OverhandStrike.Skill.IconName)
            {
                OverhandStrike.Start(sk.Duration);
                return true;
            }
            
            return false;
        }
    }
}
