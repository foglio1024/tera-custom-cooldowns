using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.ViewModels
{
    public class ValkyrieBarManager : ClassManager
    {
        private static ValkyrieBarManager _instance;
        public static ValkyrieBarManager Instance => _instance ?? (_instance = new ValkyrieBarManager());
        public Counter RunemarksCounter { get; set; }
        public DurationCooldownIndicator Ragnarok { get; private set; }

        public ValkyrieBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            RunemarksCounter = new Counter(7, false);
            LoadSpecialSkills();
        }

        protected override void LoadSpecialSkills()
        {
            //Ragnarok
            Ragnarok = new DurationCooldownIndicator(Dispatcher);
            SkillsDatabase.TryGetSkill(120100, Class.Glaiver, out Skill rag);
            Ragnarok.Cooldown = new FixedSkillCooldown(rag, CooldownType.Skill, Dispatcher);
            Ragnarok.Buff = new FixedSkillCooldown(rag, CooldownType.Skill, Dispatcher);
        }
        public override bool StartSpecialSkill(SkillCooldown sk)
        {

            if (sk.Skill.IconName == Ragnarok.Cooldown.Skill.IconName)
            {
                Ragnarok.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}