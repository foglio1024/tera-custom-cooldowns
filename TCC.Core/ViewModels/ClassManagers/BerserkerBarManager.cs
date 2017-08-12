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
    public class BerserkerBarManager : ClassManager
    {
        private static BerserkerBarManager _instance;
        public static BerserkerBarManager Instance => _instance ?? (_instance = new BerserkerBarManager());

        public DurationCooldownIndicator FieryRage { get; set; }
        public DurationCooldownIndicator Bloodlust { get; set; }
        


        public BerserkerBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSpecialSkills();
        }

        protected override void LoadSpecialSkills()
        {
            SkillsDatabase.TryGetSkill(80600, Class.Berserker, out Skill fr);
            SkillsDatabase.TryGetSkill(210200, Class.Berserker, out Skill bl);
            FieryRage = new DurationCooldownIndicator(_dispatcher)
            {
                Cooldown = new FixedSkillCooldown(fr, CooldownType.Skill, _dispatcher, true),
                Buff = new FixedSkillCooldown(fr, CooldownType.Skill, _dispatcher, true)
            };
            Bloodlust = new DurationCooldownIndicator(_dispatcher)
            {
                Cooldown = new FixedSkillCooldown(bl, CooldownType.Skill, _dispatcher, true),
                Buff = new FixedSkillCooldown(bl, CooldownType.Skill, _dispatcher, true)
            };
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == FieryRage.Cooldown.Skill.IconName)
            {
                FieryRage.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Bloodlust.Cooldown.Skill.IconName)
            {
                Bloodlust.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
