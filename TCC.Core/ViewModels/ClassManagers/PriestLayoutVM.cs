using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers
{
    public class PriestLayoutVM : BaseClassLayoutVM
    {
        public SkillWithEffect EnergyStars { get; }
        public SkillWithEffect Grace { get; }
        public SkillWithEffect EdictOfJudgment { get; }
        public SkillWithEffect DivineCharge { get; }
        public SkillWithEffect TripleNemesis { get; }


        public PriestLayoutVM()
        {
            //Energy Stars
            Game.DB!.SkillsDatabase.TryGetSkill(350410, Class.Priest, out var es);
            EnergyStars = new SkillWithEffect(_dispatcher, es);

            // Grace
            Game.DB.SkillsDatabase.TryGetSkill(390100, Class.Priest, out var gr);
            Grace = new SkillWithEffect(_dispatcher, gr);

            Grace.Effect.Started += OnGraceBuffStarted;
            Grace.Effect.Ended += OnGraceBuffEnded;

            // Edict Of Judgment
            Game.DB.SkillsDatabase.TryGetSkill(430100, Class.Priest, out var ed);
            EdictOfJudgment = new SkillWithEffect(_dispatcher, ed);
            EdictOfJudgment.Effect.Started += OnEdictBuffStarted;
            EdictOfJudgment.Effect.Ended += OnEdictBuffEnded;

            // Divine Charge
            Game.DB.SkillsDatabase.TryGetSkill(280200, Class.Priest, out var dc);
            DivineCharge = new SkillWithEffect(_dispatcher, dc);

            // Tripple Nenesis
            Game.DB.SkillsDatabase.TryGetSkill(290100, Class.Priest, out var tn);
            TripleNemesis = new SkillWithEffect(_dispatcher, tn);

            AbnormalityTracker.MarkingExpired += OnTripleNemesisExpired;
            AbnormalityTracker.MarkingRefreshed += OnTripleNemesisRefreshed;
        }

        private void OnTripleNemesisRefreshed(ulong duration)
        {
            TripleNemesis.RefreshEffect(duration);
            TripleNemesis.Cooldown.FlashOnAvailable = false;

        }

        private void OnTripleNemesisExpired()
        {
            TripleNemesis.StopEffect();
            TripleNemesis.Cooldown.FlashOnAvailable = true;

        }

        private void OnGraceBuffEnded(CooldownMode obj) => Grace.Cooldown.FlashOnAvailable = true;
        private void OnGraceBuffStarted(ulong cd, CooldownMode obj) => Grace.Cooldown.FlashOnAvailable = false;
        private void OnEdictBuffEnded(CooldownMode obj) => EdictOfJudgment.Cooldown.FlashOnAvailable = true;
        private void OnEdictBuffStarted(ulong cd, CooldownMode obj) => EdictOfJudgment.Cooldown.FlashOnAvailable = false;

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == EnergyStars.Cooldown.Skill.IconName)
            {
                EnergyStars.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == Grace.Cooldown.Skill.IconName)
            {
                Grace.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == EdictOfJudgment.Cooldown.Skill.IconName)
            {
                EdictOfJudgment.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == DivineCharge.Cooldown.Skill.IconName)
            {
                DivineCharge.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != TripleNemesis.Cooldown.Skill.IconName) return false;
            TripleNemesis.StartCooldown(sk.Duration);
            return true;

        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            if (skill.IconName != EdictOfJudgment.Cooldown.Skill.IconName) return false;
            EdictOfJudgment.RefreshCooldown(cd, skill.Id);
            return true;

        }

        public override void Dispose()
        {
            Grace.Dispose();
            EdictOfJudgment.Dispose();
            DivineCharge.Dispose();
            TripleNemesis.Dispose();
        }
    }
}