using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public class ReaperBarManager : ClassManager
    {

        public DurationCooldownIndicator ShadowReaping { get; set; }
        public DurationCooldownIndicator ShroudedEscape { get; set; }

        public override void LoadSpecialSkills()
        {
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(160100, Class.Reaper, out var sr);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(180100, Class.Reaper, out var se);
            ShadowReaping = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(sr, true) { CanFlash = true },
                Buff = new Cooldown(sr, true)
            };
            ShroudedEscape = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(se, true) { CanFlash = true },
                Buff = new Cooldown(se, true)
            };
        }

        public override void Dispose()
        {
            ShadowReaping.Cooldown.Dispose();
            ShroudedEscape.Cooldown.Dispose();
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == ShadowReaping.Cooldown.Skill.IconName)
            {
                ShadowReaping.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == ShroudedEscape.Cooldown.Skill.IconName)
            {
                ShroudedEscape.Cooldown.Start(sk.Duration);
                return true;
            }
            return false;
        }
    }
}
