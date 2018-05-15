using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class ArcherBarManager : ClassManager
    {
        private ArcherFocusTracker _focus;
        private StanceTracker<ArcherStance> _stance;
        private FixedSkillCooldown _thunderbolt;

        public ArcherFocusTracker Focus
        {
            get => _focus;
            set
            {
                if (_focus == value) return;
                _focus = value;
                NPC();
            }
        }
        public StanceTracker<ArcherStance> Stance
        {
            get => _stance;
            set
            {
                if(_stance== value) return;
                _stance = value;
                NPC();
            }
        }
        public FixedSkillCooldown Thunderbolt
        {
            get => _thunderbolt;
            set
            {
                if(_thunderbolt == value) return;
                _thunderbolt = value;
                NPC();
            }
        }

        public ArcherBarManager() : base()
        {
            Focus = new ArcherFocusTracker();
            Stance = new StanceTracker<ArcherStance>();
            //CurrentClassManager = this;
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(290100, Class.Archer, out var tb);
            Thunderbolt = new FixedSkillCooldown(tb, true);

        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == Thunderbolt.Skill.IconName)
            {
                Thunderbolt.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
