using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class GunnerLayoutVM : BaseClassLayoutVM
    {

        public Cooldown BurstFire { get;  }
        public Cooldown Balder { get; }
        public Cooldown Bombardment { get; }
        public SkillWithEffect ModularSystem { get; }

        public GunnerLayoutVM()
        {
            Game.DB!.SkillsDatabase.TryGetSkill(51000, Class.Gunner, out var bfire);
            Game.DB.SkillsDatabase.TryGetSkill(130200, Class.Gunner, out var balder);
            Game.DB.SkillsDatabase.TryGetSkill(20600, Class.Gunner, out var bombard);
            Game.DB.SkillsDatabase.TryGetSkill(410100, Class.Gunner, out var modSys);


            BurstFire = new Cooldown(bfire, false);
            Bombardment = new Cooldown(bombard, true) { CanFlash = true };
            Balder = new Cooldown(balder, true) { CanFlash = true };

            ModularSystem = new SkillWithEffect(Dispatcher, modSys);
            // ????
            //Balder.FlashOnAvailable = true;
            //Bombardment.FlashOnAvailable = true;
            //ModularSystem.Cooldown.FlashOnAvailable = true;

            //StaminaTracker.PropertyChanged += FlashBfIfFullWp;
        }

        public override void Dispose()
        {
            Bombardment.Dispose();
            Balder.Dispose();
            ModularSystem.Dispose();
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == Balder.Skill.IconName)
            {
                Balder.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Bombardment.Skill.IconName)
            {
                Bombardment.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != ModularSystem.Cooldown.Skill.IconName) return false;
            ModularSystem.StartCooldown(sk.Duration);
            return true;
        }
    }
}
