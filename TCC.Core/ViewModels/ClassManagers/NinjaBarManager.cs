using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class NinjaBarManager : ClassManager
    {
        private bool _focusOn;

        public FixedSkillCooldown BurningHeart { get; set; }
        public FixedSkillCooldown FireAvalanche { get; set; }

        public bool FocusOn
        {
            get { return _focusOn; }
            set
            {
                if (_focusOn == value) return;
                _focusOn = value;
                NPC(nameof(FocusOn));
            }

        }

        public NinjaBarManager() : base()
        {
        }

        private void FlashOnMaxSt(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StaminaTracker.Maxed))
            {
                BurningHeart.FlashOnAvailable = StaminaTracker.Maxed;
                FireAvalanche.FlashOnAvailable = StaminaTracker.Maxed;
            }
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(150700, Class.Ninja, out var bh);
            SessionManager.SkillsDatabase.TryGetSkill(80200, Class.Ninja, out var fa);
            BurningHeart = new FixedSkillCooldown(bh,  false);
            FireAvalanche = new FixedSkillCooldown(fa,  false);
            StaminaTracker.PropertyChanged += FlashOnMaxSt;

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
