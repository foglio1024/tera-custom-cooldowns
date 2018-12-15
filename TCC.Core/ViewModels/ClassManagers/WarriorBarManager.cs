using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{

    public class WarriorBarManager : ClassManager
    {

        public DurationCooldownIndicator DeadlyGamble { get; set; }
        public Counter EdgeCounter { get; set; }
        public Cooldown Swift { get; set; }
        public StanceTracker<WarriorStance> Stance { get; set; }
        public StatTracker TraverseCut { get; set; }
        //public StatTracker TempestAura { get; set; }
        private bool _swiftProc;

        public bool SwiftProc
        {
            get => _swiftProc;
            set
            {
                if (_swiftProc == value) return; _swiftProc = value;
                N();
            }
        }

        public WarriorBarManager()
        {
            EdgeCounter = new Counter(10, true);
            TraverseCut = new StatTracker { Max = 13, Val = 0 };
            //TempestAura = new StatTracker { Max = 50, Val = 0 };
            Stance = new StanceTracker<WarriorStance>();
        }

        public bool ShowEdge => Settings.SettingsHolder.WarriorShowEdge;
        public bool ShowTraverseCut => Settings.SettingsHolder.WarriorShowTraverseCut;
        public WarriorEdgeMode WarriorEdgeMode => Settings.SettingsHolder.WarriorEdgeMode;

        public sealed override void LoadSpecialSkills()
        {
            //Deadly gamble
            SessionManager.SkillsDatabase.TryGetSkill(200200, Class.Warrior, out var dg);
            DeadlyGamble = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(dg, false),
                Cooldown = new Cooldown(dg, true) { CanFlash = true }
            };
            var ab = SessionManager.AbnormalityDatabase.Abnormalities[21010];//21070 dfa
            Swift = new Cooldown(new Skill(ab), false);
        }

        public override void Dispose()
        {
            DeadlyGamble.Cooldown.Dispose();
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
