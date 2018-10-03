using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{
    public class ArcherBarManager : ClassManager
    {
        private ArcherFocusTracker _focus;
        private StanceTracker<ArcherStance> _stance;
        private FixedSkillCooldown _thunderbolt;
        private DurationCooldownIndicator _velikMark;

        public ArcherFocusTracker Focus
        {
            get => _focus;
            private set
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
        public DurationCooldownIndicator VelikMark
        {
            get => _velikMark;
            set
            {
                if(_velikMark == value) return;
                _velikMark = value;
                NPC();
            }
        }

        public ArcherBarManager()
        {
            Focus = new ArcherFocusTracker();
            Stance = new StanceTracker<ArcherStance>();
            AbnormalityTracker = new ArcherAbnormalityTracker();
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(290100, Class.Archer, out var tb);
            SessionManager.SkillsDatabase.TryGetSkill(120500, Class.Archer, out var vm);
            Thunderbolt = new FixedSkillCooldown(tb, true);
            VelikMark = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new FixedSkillCooldown(vm, false),
                Buff = new FixedSkillCooldown(vm, false)
            };

            ClassAbnormalityTracker.MarkingExpired += OnVelikMarkExpired;
            ClassAbnormalityTracker.MarkingRefreshed += OnVelikMarkRefreshed;
        }

        private void OnVelikMarkRefreshed(ulong duration)
        {
            VelikMark.Buff.Refresh(duration);
            VelikMark.Cooldown.FlashOnAvailable = false;

        }

        private void OnVelikMarkExpired()
        {
            VelikMark.Buff.Refresh(0);
            VelikMark.Cooldown.FlashOnAvailable = true;
        }


        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == Thunderbolt.Skill.IconName)
            {
                Thunderbolt.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == VelikMark.Cooldown.Skill.IconName)
            {
                VelikMark.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
