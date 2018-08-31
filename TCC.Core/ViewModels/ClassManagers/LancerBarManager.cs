using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{
    internal class LancerBarManager : ClassManager
    {
        public LancerBarManager() : base()
        {
            LH = new StatTracker()
            {
                Max = 10
            };
            AbnormalityTracker = new LancerAbnormalityTracker();
        }

        public DurationCooldownIndicator AdrenalineRush { get; set; }
        public DurationCooldownIndicator GuardianShout { get; set; }
        public FixedSkillCooldown Infuriate { get; set; }
        public StatTracker LH { get; set; }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == GuardianShout.Cooldown.Skill.IconName)
            {
                GuardianShout.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == AdrenalineRush.Cooldown.Skill.IconName)
            {
                AdrenalineRush.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if(sk.Skill.IconName == Infuriate.Skill.IconName)
            {
                Infuriate.Start(sk.Cooldown);
                return true;
            }
            return false;
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(70300, Class.Lancer, out var gshout);
            SessionManager.SkillsDatabase.TryGetSkill(170200, Class.Lancer, out var arush);
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Lancer, out var infu);

            GuardianShout = new DurationCooldownIndicator(_dispatcher);
            AdrenalineRush = new DurationCooldownIndicator(_dispatcher);

            GuardianShout.Cooldown = new FixedSkillCooldown(gshout,  true);
            GuardianShout.Buff = new FixedSkillCooldown(gshout,  false);
            AdrenalineRush.Cooldown = new FixedSkillCooldown(arush,  true);
            AdrenalineRush.Buff = new FixedSkillCooldown(arush,  false);
            Infuriate = new FixedSkillCooldown(infu,  true);
        }
    }
}