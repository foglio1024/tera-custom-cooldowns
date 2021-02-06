using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class ReaperLayoutVM : BaseClassLayoutVM
    {

        public SkillWithEffect ShadowReaping { get; }
        public SkillWithEffect ShroudedEscape { get; }

        public ReaperLayoutVM()
        {
            Game.DB!.SkillsDatabase.TryGetSkill(160100, Class.Reaper, out var sr);
            ShadowReaping = new SkillWithEffect(Dispatcher, sr);

            Game.DB.SkillsDatabase.TryGetSkill(180100, Class.Reaper, out var se);
            ShroudedEscape = new SkillWithEffect(Dispatcher, se);
        }

        public override void Dispose()
        {
            ShadowReaping.Dispose();
            ShroudedEscape.Dispose();
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == ShadowReaping.Cooldown.Skill.IconName)
            {
                ShadowReaping.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != ShroudedEscape.Cooldown.Skill.IconName) return false;
            ShroudedEscape.StartCooldown(sk.Duration);
            return true;
        }
    }
}
