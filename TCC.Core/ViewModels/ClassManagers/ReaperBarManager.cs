using TCC.Data;

namespace TCC.ViewModels
{
    public class ReaperBarManager : ClassManager
    {

        public DurationCooldownIndicator ShadowReaping { get; set; }
        public DurationCooldownIndicator ShroudedEscape { get; set; }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(160100, Class.Reaper, out var sr);
            SessionManager.SkillsDatabase.TryGetSkill(180100, Class.Reaper, out var se);
            ShadowReaping = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new FixedSkillCooldown(sr, true),
                Buff = new FixedSkillCooldown(sr, true)
            };
            ShroudedEscape = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown =  new FixedSkillCooldown(se, true),
                Buff = new FixedSkillCooldown(se, true)
            };
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == ShadowReaping.Cooldown.Skill.IconName)
            {
                ShadowReaping.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == ShroudedEscape.Cooldown.Skill.IconName)
            {
                ShroudedEscape.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
