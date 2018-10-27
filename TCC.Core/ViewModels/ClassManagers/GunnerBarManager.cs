using TCC.Data;

namespace TCC.ViewModels
{
    public class GunnerBarManager : ClassManager
    {

        public DurationCooldownIndicator BurstFire { get; set; }
        public DurationCooldownIndicator Balder { get; set; }
        public DurationCooldownIndicator Bombardment { get; set; }
        public DurationCooldownIndicator ModularSystem { get; set; }

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
            SessionManager.SkillsDatabase.TryGetSkill(410100, Class.Gunner, out var modSys);


            BurstFire = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(bfire, false),
                Cooldown = new Cooldown(bfire, true)
            };
            Bombardment = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(bombard, false),
                Cooldown = new Cooldown(bombard, false)
            };
            Balder = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(balder, false),
                Cooldown = new Cooldown(balder, false)
            };
            ModularSystem = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(modSys, false),
                Cooldown = new Cooldown(modSys, true)
            };
            Balder.Cooldown.FlashOnAvailable = true;
            Bombardment.Cooldown.FlashOnAvailable = true;
            ModularSystem.Cooldown.FlashOnAvailable = true;

            StaminaTracker.PropertyChanged += FlashBfIfFullWp;
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (Balder.Cooldown.Skill != null && sk.Skill.IconName == Balder.Cooldown.Skill.IconName)
            {
                Balder.Cooldown.Start(sk.Duration);
                return true;
            }
            if (Bombardment.Cooldown.Skill != null && sk.Skill.IconName == Bombardment.Cooldown.Skill.IconName)
            {
                Bombardment.Cooldown.Start(sk.Duration);
                return true;
            }
            if (ModularSystem.Cooldown.Skill != null && sk.Skill.IconName == ModularSystem.Cooldown.Skill.IconName)
            {
                ModularSystem.Cooldown.Start(sk.Duration);
                return true;
            }
            return false;
        }
    }
}
