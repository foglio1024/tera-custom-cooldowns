using TCC.Data;

namespace TCC.ViewModels
{
    public class GunnerBarManager : ClassManager
    {

        public DurationCooldownIndicator BurstFire { get; set; }
        public DurationCooldownIndicator Balder { get; set; }
        public DurationCooldownIndicator Bombardment { get; set; }

        private void FlashBfIfFullWp(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StaminaTracker.Val))
            {
                if (StaminaTracker.Factor > .8) BurstFire.Cooldown.ForceEnded();
                else BurstFire.Cooldown.ForceStopFlashing();
            }
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(51000, Class.Gunner, out var bfire);
            SessionManager.SkillsDatabase.TryGetSkill(130200, Class.Gunner, out var balder);
            SessionManager.SkillsDatabase.TryGetSkill(20600, Class.Gunner, out var bombard);

            BurstFire = new DurationCooldownIndicator(Dispatcher);
            Balder = new DurationCooldownIndicator(Dispatcher);
            Bombardment = new DurationCooldownIndicator(Dispatcher);

            BurstFire.Buff = new FixedSkillCooldown(bfire, false);
            Balder.Buff = new FixedSkillCooldown(balder, false);
            Bombardment.Buff = new FixedSkillCooldown(bombard, false);
            BurstFire.Cooldown = new FixedSkillCooldown(bfire, true);
            Balder.Cooldown = new FixedSkillCooldown(balder, false);
            Bombardment.Cooldown = new FixedSkillCooldown(bombard, false);
            StaminaTracker.PropertyChanged += FlashBfIfFullWp;
            Balder.Cooldown.FlashOnAvailable = true;
            Bombardment.Cooldown.FlashOnAvailable = true;

        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (Balder.Cooldown.Skill != null && sk.Skill.IconName == Balder.Cooldown.Skill.IconName)
            {
                Balder.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (Bombardment.Cooldown.Skill != null && sk.Skill.IconName == Bombardment.Cooldown.Skill.IconName)
            {
                Bombardment.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
