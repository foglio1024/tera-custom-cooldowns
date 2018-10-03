using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{
    public class ReaperBarManager : ClassManager
    {

        public DurationCooldownIndicator ShadowReaping { get; set; }
        public ReaperBarManager()
        {
            AbnormalityTracker = new ReaperAbnormalityTracker();
        }
        public override void LoadSpecialSkills()
        {
            ShadowReaping = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(160100, Class.Reaper, out var sr);
            ShadowReaping.Cooldown = new FixedSkillCooldown(sr, true);
            ShadowReaping.Buff= new FixedSkillCooldown(sr, true);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == ShadowReaping.Cooldown.Skill.IconName)
            {
                ShadowReaping.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
