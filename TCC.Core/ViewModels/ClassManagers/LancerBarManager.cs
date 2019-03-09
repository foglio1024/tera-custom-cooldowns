using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    internal class LancerBarManager : ClassManager
    {
        public LancerBarManager()
        {
            LH = new LancerLineHeldTracker();
        }

        public DurationCooldownIndicator AdrenalineRush { get; set; }
        public DurationCooldownIndicator GuardianShout { get; set; }
        public Cooldown Infuriate { get; set; }
        public LancerLineHeldTracker LH { get; set; }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == GuardianShout.Cooldown.Skill.IconName)
            {
                GuardianShout.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == AdrenalineRush.Cooldown.Skill.IconName)
            {
                AdrenalineRush.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Infuriate.Skill.IconName)
            {
                Infuriate.Start(sk.Duration);
                return true;
            }
            return false;
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(70300, Class.Lancer, out var gshout);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(170200, Class.Lancer, out var arush);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(120100, Class.Lancer, out var infu);

            GuardianShout = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(gshout, true) { CanFlash = true },
                Buff = new Cooldown(gshout, false)
            };
            AdrenalineRush = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(arush, true) { CanFlash = true },
                Buff = new Cooldown(arush, false)
            };

            Infuriate = new Cooldown(infu, true) { CanFlash = true };
        }

        public override void Dispose()
        {
            GuardianShout.Cooldown.Dispose();
            AdrenalineRush.Cooldown.Dispose();
            Infuriate.Dispose();
        }
    }
}