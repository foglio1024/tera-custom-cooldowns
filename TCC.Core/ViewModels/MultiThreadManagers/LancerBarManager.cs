using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;

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
        private static LancerBarManager _instance;
        public static LancerBarManager Instance => _instance ?? (_instance = new LancerBarManager());
        public LancerBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
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

            SkillsDatabase.TryGetSkill(70300, Class.Lancer, out Skill gshout);
            SkillsDatabase.TryGetSkill(170200, Class.Lancer, out Skill arush);
            SkillsDatabase.TryGetSkill(120100, Class.Lancer, out Skill infu);

            GuardianShout = new DurationCooldownIndicator(Dispatcher);
            AdrenalineRush = new DurationCooldownIndicator(Dispatcher);
            Infuriate = new DurationCooldownIndicator(Dispatcher);

            GuardianShout.Cooldown = new FixedSkillCooldown(gshout, CooldownType.Skill, Dispatcher);
            GuardianShout.Buff = new FixedSkillCooldown(gshout, CooldownType.Skill, Dispatcher);
            AdrenalineRush.Cooldown = new FixedSkillCooldown(arush, CooldownType.Skill, Dispatcher);
            AdrenalineRush.Buff = new FixedSkillCooldown(arush, CooldownType.Skill, Dispatcher);
            Infuriate.Cooldown = new FixedSkillCooldown(infu, CooldownType.Skill, Dispatcher);
        }
    }
}