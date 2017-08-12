using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class SlayerBarManager : ClassManager
    {
        private static SlayerBarManager _instance;
        public static SlayerBarManager Instance => _instance ?? (_instance = new SlayerBarManager());

        public DurationCooldownIndicator InColdBlood { get; set; }

        public SlayerBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSpecialSkills();
        }

        protected override void LoadSpecialSkills()
        {
            SkillsDatabase.TryGetSkill(200200, Class.Slayer, out Skill icb);
            InColdBlood =
                new DurationCooldownIndicator(_dispatcher)
                {
                    Buff = new FixedSkillCooldown(icb, CooldownType.Skill, _dispatcher, false),
                    Cooldown = new FixedSkillCooldown(icb, CooldownType.Skill, _dispatcher, true)
                };
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == InColdBlood.Cooldown.Skill.IconName)
            {
                InColdBlood.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
