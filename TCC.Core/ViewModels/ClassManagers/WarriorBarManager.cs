using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{

    public class WarriorBarManager : ClassManager
    {
        private static WarriorBarManager _instance;
        public static WarriorBarManager Instance => _instance ?? (_instance = new WarriorBarManager());

        public DurationCooldownIndicator DeadlyGamble { get; set; }

        public Counter EdgeCounter { get; set; }
        public StanceTracker<WarriorStance> Stance { get; set; }

        public WarriorBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            EdgeCounter = new Counter(10, true);
            Stance = new StanceTracker<WarriorStance>();
            //LoadSkills("warrior-skills.xml", Class.Warrior);
            LoadSpecialSkills();
        }

        protected override void LoadSpecialSkills()
        {
            //Deadly gamble
            DeadlyGamble = new DurationCooldownIndicator(Dispatcher);
            SkillsDatabase.TryGetSkill(200200, Class.Warrior, out Skill dg);
            DeadlyGamble.Buff = new FixedSkillCooldown(dg, CooldownType.Skill, Dispatcher, false);
            DeadlyGamble.Cooldown = new FixedSkillCooldown(dg, CooldownType.Skill, Dispatcher, true);
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
