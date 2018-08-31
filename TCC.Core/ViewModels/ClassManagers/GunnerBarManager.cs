using TCC.Data;

namespace TCC.ViewModels
{
    public class GunnerBarManager : ClassManager
    {

        public DurationCooldownIndicator BurstFire { get; set; }
        public DurationCooldownIndicator Balder { get; set; }
        public DurationCooldownIndicator Bombardment { get; set; }
        public GunnerBarManager() : base()
        {
        }

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
            SessionManager.SkillsDatabase.TryGetSkill(20700, Class.Gunner, out var bombard);

            BurstFire = new DurationCooldownIndicator(_dispatcher);
            Balder = new DurationCooldownIndicator(_dispatcher);
            Bombardment = new DurationCooldownIndicator(_dispatcher);

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
            if (sk.Skill.IconName == Balder.Cooldown.Skill.IconName)
            {
                Balder.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Bombardment.Cooldown.Skill.IconName)
            {
                Bombardment.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
