using TCC.ClassSpecific;
using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public class PriestBarManager : ClassManager
    {
        private DurationCooldownIndicator _energyStars;

        public DurationCooldownIndicator EnergyStars
        {
            get => _energyStars;
            set
            {
                if (_energyStars == value) return;
                _energyStars = value;
                NPC();
            }
        }

        public DurationCooldownIndicator Grace { get; set; }
        public DurationCooldownIndicator EdictOfJudgment { get; set; }
        public DurationCooldownIndicator DivineCharge { get; set; }
        public DurationCooldownIndicator TripleNemesis { get; set; }


        public sealed override void LoadSpecialSkills()
        {
            //Energy Stars
            SessionManager.SkillsDatabase.TryGetSkill(350410, Class.Priest, out var es);
            EnergyStars = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(es, false),
                Cooldown = new Cooldown(es, true) {CanFlash = true}
            };

            SessionManager.SkillsDatabase.TryGetSkill(390100, Class.Priest, out var gr);
            Grace = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(gr, false),
                Cooldown = new Cooldown(gr, true) {CanFlash = true}
            };

            Grace.Buff.Started += OnGraceBuffStarted;
            Grace.Buff.Ended += OnGraceBuffEnded;

            // Edict Of Judgment
            SessionManager.SkillsDatabase.TryGetSkill(430100, Class.Priest, out var ed);
            EdictOfJudgment = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(ed, false),
                Cooldown = new Cooldown(ed, true) {CanFlash = true}
            };

            EdictOfJudgment.Buff.Started += OnEdictBuffStarted;
            EdictOfJudgment.Buff.Ended += OnEdictBuffEnded;

            // Divine Charge
            SessionManager.SkillsDatabase.TryGetSkill(280200, Class.Priest, out var dc);
            DivineCharge = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(dc, true) {CanFlash = true}
            };

            // Tripple Nenesis
            SessionManager.SkillsDatabase.TryGetSkill(290100, Class.Priest, out var tn);
            TripleNemesis = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(tn, false) {CanFlash = true},
                Buff = new Cooldown(tn, false)
            };

            ClassAbnormalityTracker.MarkingExpired += OnTripleNemesisExpired;
            ClassAbnormalityTracker.MarkingRefreshed += OnTripleNemesisRefreshed;
        }

        private void OnTripleNemesisRefreshed(ulong duration)
        {
            TripleNemesis.Buff.Refresh(duration, CooldownMode.Normal);
            TripleNemesis.Cooldown.FlashOnAvailable = false;

        }

        private void OnTripleNemesisExpired()
        {
            TripleNemesis.Buff.Refresh(0, CooldownMode.Normal);
            TripleNemesis.Cooldown.FlashOnAvailable = true;

        }

        private void OnGraceBuffEnded(CooldownMode obj) => Grace.Cooldown.FlashOnAvailable = true;
        private void OnGraceBuffStarted(CooldownMode obj) => Grace.Cooldown.FlashOnAvailable = false;

        private void OnEdictBuffEnded(CooldownMode obj) => EdictOfJudgment.Cooldown.FlashOnAvailable = true;
        private void OnEdictBuffStarted(CooldownMode obj) => EdictOfJudgment.Cooldown.FlashOnAvailable = false;

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == EnergyStars.Cooldown.Skill.IconName)
            {
                EnergyStars.Cooldown.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == Grace.Cooldown.Skill.IconName)
            {
                Grace.Cooldown.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == EdictOfJudgment.Cooldown.Skill.IconName)
            {
                EdictOfJudgment.Cooldown.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == DivineCharge.Cooldown.Skill.IconName)
            {
                DivineCharge.Cooldown.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == TripleNemesis.Cooldown.Skill.IconName)
            {
                TripleNemesis.Cooldown.Start(sk.Duration);
                return true;
            }

            return false;
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            if (skill.IconName == EdictOfJudgment.Cooldown.Skill.IconName)
            {
                EdictOfJudgment.Cooldown.Refresh(skill.Id, cd, CooldownMode.Normal);
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
            Grace.Cooldown.Dispose();
            EdictOfJudgment.Cooldown.Dispose();
            DivineCharge.Cooldown.Dispose();
            TripleNemesis.Cooldown.Dispose();
        }
    }
}