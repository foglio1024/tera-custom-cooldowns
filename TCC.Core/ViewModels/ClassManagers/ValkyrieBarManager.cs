using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class ValkyrieBarManager : ClassManager
    {
        private static ValkyrieBarManager _instance;
        public static ValkyrieBarManager Instance => _instance ?? (_instance = new ValkyrieBarManager());
        public Counter RunemarksCounter { get; set; }
        public DurationCooldownIndicator Ragnarok { get; private set; }

        public ValkyrieBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            RunemarksCounter = new Counter(7, false);
            LoadSpecialSkills();
        }

        protected override void LoadSpecialSkills()
        {
            //Ragnarok
            Ragnarok = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Glaiver, out var rag);
            Ragnarok.Cooldown = new FixedSkillCooldown(rag, _dispatcher, true);
            Ragnarok.Buff = new FixedSkillCooldown(rag, _dispatcher, false);
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