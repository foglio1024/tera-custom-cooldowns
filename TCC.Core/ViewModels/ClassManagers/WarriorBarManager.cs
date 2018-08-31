using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{

    public class WarriorBarManager : ClassManager
    {

        public DurationCooldownIndicator DeadlyGamble { get; set; }
        public Counter EdgeCounter { get; set; }
        public StanceTracker<WarriorStance> Stance { get; set; }
        public StatTracker TraverseCut { get; set; }
        //public StatTracker TempestAura { get; set; }
        public WarriorBarManager()
        {
            EdgeCounter = new Counter(10, true);
            TraverseCut = new StatTracker { Max = 13, Val = 0 };
            //TempestAura = new StatTracker { Max = 50, Val = 0 };
            Stance = new StanceTracker<WarriorStance>();
            AbnormalityTracker = new WarriorAbnormalityTracker();
        }

        public bool ShowEdge => Settings.WarriorShowEdge;
        public bool ShowTraverseCut => Settings.WarriorShowTraverseCut;
        public WarriorEdgeMode WarriorEdgeMode => Settings.WarriorEdgeMode;

        public sealed override void LoadSpecialSkills()
        {
            //Deadly gamble
            DeadlyGamble = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(200200, Class.Warrior, out var dg);
            DeadlyGamble.Buff = new FixedSkillCooldown(dg, false);
            DeadlyGamble.Cooldown = new FixedSkillCooldown(dg, true);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == DeadlyGamble.Cooldown.Skill.IconName)
            {
                DeadlyGamble.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
