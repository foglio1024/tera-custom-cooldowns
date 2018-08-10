using System;
using System.Threading.Tasks;
using System.Timers;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{

    public class WarriorBarManager : ClassManager
    {

        public DurationCooldownIndicator DeadlyGamble { get; set; }
        public Counter EdgeCounter { get; set; }
        public StanceTracker<WarriorStance> Stance { get; set; }
        public StatTracker TraverseCut { get; set; }
        public StatTracker TempestAura { get; set; }
        public WarriorBarManager() : base()
        {
            EdgeCounter = new Counter(10, true);
            TraverseCut = new StatTracker { Max = 13, Val = 0 };
            TempestAura = new StatTracker { Max = 50, Val = 0 };
            Stance = new StanceTracker<WarriorStance>();

            //Task.Delay(1000).ContinueWith(t =>
            //{
            //    var tm = new Timer(200);
            //    tm.Elapsed += (_, __) => EdgeCounter.Val = EdgeCounter.Val == 10 ? 0 : EdgeCounter.Val + 1;
            //    tm.Start();
            //});
        }

        public bool ShowEdge => SettingsManager.WarriorShowEdge;
        public bool ShowTraverseCut => SettingsManager.WarriorShowTraverseCut;

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
