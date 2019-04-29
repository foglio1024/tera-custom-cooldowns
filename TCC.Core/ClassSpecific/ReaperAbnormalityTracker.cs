using TCC.Data;
using TCC.Data.Skills;
using TCC.Parsing.Messages;
using TCC.Utilities.Extensions;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class ReaperAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int ShadowReapingId = 10151010;
        private const int ShadowStepId = 10151000;
        private const int DeathSpiralId = 10151131;
        private const int AssassinateId = 10151192;

        private readonly Skill _shadowStep;
        private readonly Skill _deathSpiral;

        public ReaperAbnormalityTracker()
        {
            SessionManager.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.chainbrandish_tex", SessionManager.CurrentPlayer.Class, out _deathSpiral);
            SessionManager.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.instantleap_tex", SessionManager.CurrentPlayer.Class, out _shadowStep);

        }
        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckShadowReaping(p);
            CheckShadowStep(p);
            CheckDeathSpiral(p);
            CheckAssassinate(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckShadowReaping(p);
            CheckAssassinate(p);

        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckShadowReaping(p);
            CheckAssassinate(p);

        }
        private void CheckDeathSpiral(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != DeathSpiralId) return;
            StartPrecooldown(_deathSpiral, p.Duration);
        }
        private void CheckShadowStep(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != ShadowStepId) return;
            StartPrecooldown(_shadowStep, p.Duration);
        }


        private static void CheckAssassinate(S_ABNORMALITY_BEGIN p)
        {
            if (AssassinateId != p.AbnormalityId) return;
            Utils.CurrentClassVM<ReaperLayoutVM>().ShroudedEscape.Buff.Start(p.Duration);
        }
        private static void CheckAssassinate(S_ABNORMALITY_REFRESH p)
        {
            if (AssassinateId != p.AbnormalityId) return;
            Utils.CurrentClassVM<ReaperLayoutVM>().ShroudedEscape.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckAssassinate(S_ABNORMALITY_END p)
        {
            if (AssassinateId != p.AbnormalityId) return;
            Utils.CurrentClassVM<ReaperLayoutVM>().ShroudedEscape.Buff.Refresh(0, CooldownMode.Normal);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_BEGIN p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            Utils.CurrentClassVM<ReaperLayoutVM>().ShadowReaping.Buff.Start(p.Duration);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_REFRESH p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            Utils.CurrentClassVM<ReaperLayoutVM>().ShadowReaping.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_END p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            Utils.CurrentClassVM<ReaperLayoutVM>().ShadowReaping.Buff.Refresh(0, CooldownMode.Normal);
        }

    }
}
