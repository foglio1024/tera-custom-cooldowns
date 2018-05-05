using System;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{

    public class WarriorBarManager : ClassManager
    {

        public DurationCooldownIndicator DeadlyGamble { get; set; }
        public Counter EdgeCounter { get; set; }
        public StanceTracker<WarriorStance> Stance { get; set; }

        public WarriorBarManager() : base()
        {
            EdgeCounter = new Counter(10, true);
            Stance = new StanceTracker<WarriorStance>();
        }

        public sealed override void LoadSpecialSkills()
        {
            //Deadly gamble
            DeadlyGamble = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(200200, Class.Warrior, out var dg);
            DeadlyGamble.Buff = new FixedSkillCooldown(dg, _dispatcher, false);
            DeadlyGamble.Cooldown = new FixedSkillCooldown(dg, _dispatcher, true);
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
