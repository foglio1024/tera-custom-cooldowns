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
    public class NinjaBarManager : ClassManager
    {
        private static NinjaBarManager _instance;
        private bool _focusOn;
        public static NinjaBarManager Instance => _instance ?? (_instance = new NinjaBarManager());

        public FixedSkillCooldown BurningHeart { get; set; }
        public FixedSkillCooldown FireAvalanche { get; set; }

        public bool FocusOn
        {
            get { return _focusOn; }
            set
            {
                if (_focusOn == value) return;
                _focusOn = value;
                NotifyPropertyChanged(nameof(FocusOn));
            }

        }

        public NinjaBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSpecialSkills();
        }
        protected override void LoadSpecialSkills()
        {
            SkillsDatabase.TryGetSkill(150700, Class.Assassin, out Skill bh);
            SkillsDatabase.TryGetSkill(80200, Class.Assassin, out Skill fa);
            BurningHeart = new FixedSkillCooldown(bh, CooldownType.Skill, _dispatcher, true);
            FireAvalanche = new FixedSkillCooldown(fa, CooldownType.Skill, _dispatcher, true);

        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == FireAvalanche.Skill.IconName)
            {
                FireAvalanche.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == BurningHeart.Skill.IconName)
            {
                BurningHeart.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
