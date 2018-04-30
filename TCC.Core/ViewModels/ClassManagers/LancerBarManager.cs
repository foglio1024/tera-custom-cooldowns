using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class DurationCooldownIndicator : TSPropertyChanged
    {
        public FixedSkillCooldown Cooldown { get; set; }
        public FixedSkillCooldown Buff { get; set; }

        public DurationCooldownIndicator(Dispatcher d)
        {
            _dispatcher = d;
            Cooldown = new FixedSkillCooldown(_dispatcher);
            Buff = new FixedSkillCooldown(_dispatcher);
        }
    }
    internal class LancerBarManager : ClassManager
    {
        public LancerBarManager() : base()
        {
            LoadSpecialSkills();
            LH = new StatTracker()
            {
                Max = 10
            };
        }

        public DurationCooldownIndicator AdrenalineRush { get; set; }
        public DurationCooldownIndicator GuardianShout { get; set; }
        public DurationCooldownIndicator Infuriate { get; set; }
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
            if(sk.Skill.IconName == Infuriate.Cooldown.Skill.IconName)
            {
                Infuriate.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }

        protected override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(70300, Class.Lancer, out var gshout);
            SessionManager.SkillsDatabase.TryGetSkill(170200, Class.Lancer, out var arush);
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Lancer, out var infu);

            GuardianShout = new DurationCooldownIndicator(_dispatcher);
            AdrenalineRush = new DurationCooldownIndicator(_dispatcher);
            Infuriate = new DurationCooldownIndicator(_dispatcher);

            GuardianShout.Cooldown = new FixedSkillCooldown(gshout, _dispatcher, true);
            GuardianShout.Buff = new FixedSkillCooldown(gshout, _dispatcher, false);
            AdrenalineRush.Cooldown = new FixedSkillCooldown(arush, _dispatcher, true);
            AdrenalineRush.Buff = new FixedSkillCooldown(arush, _dispatcher, false);
            Infuriate.Cooldown = new FixedSkillCooldown(infu, _dispatcher, true);
        }
    }
}