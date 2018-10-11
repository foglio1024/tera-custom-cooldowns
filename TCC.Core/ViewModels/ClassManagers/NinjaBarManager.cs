using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{
    public class NinjaBarManager : ClassManager
    {
        private bool _focusOn;

        public FixedSkillCooldown BurningHeart { get; set; }
        public FixedSkillCooldown FireAvalanche { get; set; }
        public DurationCooldownIndicator InnerHarmony { get; set; }

        public bool FocusOn
        {
            get => _focusOn;
            set
            {
                if (_focusOn == value) return;
                _focusOn = value;
                NPC(nameof(FocusOn));
            }

        }

        public NinjaBarManager()
        {
            AbnormalityTracker = new NinjaAbnormalityTracker();
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
            SessionManager.SkillsDatabase.TryGetSkill(230100, Class.Ninja, out var ih);
            BurningHeart = new FixedSkillCooldown(bh,  false);
            FireAvalanche = new FixedSkillCooldown(fa,  false);

            InnerHarmony = new DurationCooldownIndicator(Dispatcher);
            InnerHarmony.Cooldown = new FixedSkillCooldown(ih, true);
            InnerHarmony.Buff = new FixedSkillCooldown(ih, false);

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
            if (sk.Skill.IconName == InnerHarmony.Cooldown.Skill.IconName)
            {
                InnerHarmony.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
