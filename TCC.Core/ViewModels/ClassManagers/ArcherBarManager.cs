using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class ArcherBarManager : ClassManager
    {
        private static ArcherBarManager _instance;
        public static ArcherBarManager Instance => _instance ?? (_instance = new ArcherBarManager());

        public ArcherFocusTracker Focus { get; set; }
        public StanceTracker<ArcherStance> Stance { get; set; }
        public FixedSkillCooldown Thunderbolt { get; set; }
        public ArcherBarManager() : base()
        {
            _instance = this;
            Focus = new ArcherFocusTracker();
            Stance = new StanceTracker<ArcherStance>();
            LoadSpecialSkills();
            CurrentClassManager = this;
        }

        protected override void LoadSpecialSkills()
        {
            SkillsDatabase.TryGetSkill(290100, Class.Archer, out Skill tb);
            Thunderbolt = new FixedSkillCooldown(tb, CooldownType.Skill, _dispatcher, true);

        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if(sk.Skill.IconName == Thunderbolt.Skill.IconName)
            {
                Thunderbolt.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
