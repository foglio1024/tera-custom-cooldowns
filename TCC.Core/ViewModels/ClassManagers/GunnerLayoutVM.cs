using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class GunnerLayoutVM : BaseClassLayoutVM
    {

        public Cooldown BurstFire { get; set; }
        public Cooldown Balder { get; set; }
        public Cooldown Bombardment { get; set; }
        public DurationCooldownIndicator ModularSystem { get; set; }

        public override void LoadSpecialSkills()
        {
            Session.DB.SkillsDatabase.TryGetSkill(51000, Class.Gunner, out var bfire);
            Session.DB.SkillsDatabase.TryGetSkill(130200, Class.Gunner, out var balder);
            Session.DB.SkillsDatabase.TryGetSkill(20600, Class.Gunner, out var bombard);
            Session.DB.SkillsDatabase.TryGetSkill(410100, Class.Gunner, out var modSys);


            BurstFire = new Cooldown(bfire, true);
            Bombardment = new Cooldown(bombard, false) { CanFlash = true };
            Balder = new Cooldown(balder, false) { CanFlash = true };

            ModularSystem = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(modSys, false),
                Cooldown = new Cooldown(modSys, true) { CanFlash = true }
            };
            Balder.FlashOnAvailable = true;
            Bombardment.FlashOnAvailable = true;
            ModularSystem.Cooldown.FlashOnAvailable = true;

            //StaminaTracker.PropertyChanged += FlashBfIfFullWp;
        }

        public override void Dispose()
        {
            Bombardment.Dispose();
            Balder.Dispose();
            ModularSystem.Cooldown.Dispose();
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (Balder.Skill != null && sk.Skill.IconName == Balder.Skill.IconName)
            {
                Balder.Start(sk.Duration);
                return true;
            }
            if (Bombardment.Skill != null && sk.Skill.IconName == Bombardment.Skill.IconName)
            {
                Bombardment.Start(sk.Duration);
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
