using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Databases;
using Tera.Game;

namespace TCC.ViewModels
{
    public class ReaperBarManager : ClassManager
    {
        private static ReaperBarManager _instance;
        public static ReaperBarManager Instance => _instance ?? (_instance = new ReaperBarManager());

        public DurationCooldownIndicator ShadowReaping { get; set; }
        public ReaperBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSpecialSkills();
        }
        protected override void LoadSpecialSkills()
        {
            ShadowReaping = new DurationCooldownIndicator(_dispatcher);
            SkillsDatabase.TryGetSkill(160100, Class.Soulless, out Skill sr);
            ShadowReaping.Cooldown = new FixedSkillCooldown(sr, CooldownType.Skill, _dispatcher, true);
            ShadowReaping.Buff= new FixedSkillCooldown(sr, CooldownType.Skill, _dispatcher, true);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == ShadowReaping.Cooldown.Skill.IconName)
            {
                ShadowReaping.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
