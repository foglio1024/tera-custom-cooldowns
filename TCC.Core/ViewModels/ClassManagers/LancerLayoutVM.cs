using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers
{
    internal class LancerLayoutVM : BaseClassLayoutVM
    {
        public SkillWithEffect AdrenalineRush { get;  }
        public SkillWithEffect GuardianShout { get; }
        public Cooldown Infuriate { get; }
        public LancerLineHeldTracker LH { get; }
        public LancerLayoutVM()
        {
            LH = new LancerLineHeldTracker();
            Game.Me.Death += OnDeath;
            Game.DB!.SkillsDatabase.TryGetSkill(70300, Class.Lancer, out var gshout);
            GuardianShout = new SkillWithEffect(Dispatcher, gshout);

            Game.DB.SkillsDatabase.TryGetSkill(170200, Class.Lancer, out var arush);
            AdrenalineRush = new SkillWithEffect(Dispatcher, arush);

            Game.DB.SkillsDatabase.TryGetSkill(120100, Class.Lancer, out var infu);
            Infuriate = new Cooldown(infu, true) { CanFlash = true };

        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == GuardianShout.Cooldown.Skill.IconName)
            {
                GuardianShout.StartCooldown(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == AdrenalineRush.Cooldown.Skill.IconName)
            {
                AdrenalineRush.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != Infuriate.Skill.IconName) return false;
            Infuriate.Start(sk.Duration);
            return true;
        }

        private void OnDeath()
        {
            LH.Stop();
            GuardianShout.StopEffect();
            AdrenalineRush.StopEffect();
        }

        public override void Dispose()
        {
            GuardianShout.Dispose();
            AdrenalineRush.Dispose();
            Infuriate.Dispose();
        }
    }
}