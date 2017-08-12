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
    public class SorcererBarManager : ClassManager
    {
        private static SorcererBarManager _instance;
        public static SorcererBarManager Instance => _instance ?? (_instance = new SorcererBarManager());
        public DurationCooldownIndicator ManaBoost { get; set; }

        public SorcererBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSpecialSkills();

        }
        protected override void LoadSpecialSkills()
        {
            ManaBoost = new DurationCooldownIndicator(_dispatcher);
            SkillsDatabase.TryGetSkill(340200, Class.Sorcerer, out Skill mb);
            ManaBoost.Cooldown = new FixedSkillCooldown(mb, CooldownType.Skill, _dispatcher, true);
            ManaBoost.Buff = new FixedSkillCooldown(mb, CooldownType.Skill, _dispatcher, false);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == ManaBoost.Cooldown.Skill.IconName)
            {
                ManaBoost.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
