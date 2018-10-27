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
        }

        public bool ShowEdge => Settings.WarriorShowEdge;
        public bool ShowTraverseCut => Settings.WarriorShowTraverseCut;
        public WarriorEdgeMode WarriorEdgeMode => Settings.WarriorEdgeMode;

        public sealed override void LoadSpecialSkills()
        {
            //Deadly gamble
            DeadlyGamble = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(200200, Class.Warrior, out var dg);
            DeadlyGamble.Buff = new Cooldown(dg, false);
            DeadlyGamble.Cooldown = new Cooldown(dg, true);
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == DeadlyGamble.Cooldown.Skill.IconName)
            {
                DeadlyGamble.Cooldown.Start(sk.Duration);
                return true;
            }
            return false;
        }
    }
}
