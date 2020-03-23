using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    internal class LancerLayoutVM : BaseClassLayoutVM
    {
        public LancerLayoutVM()
        {
            LH = new LancerLineHeldTracker();
            Game.Me.Death += OnDeath;
        }

        
        private void OnDeath()
        {
            LH.Stop();
            GuardianShout.Buff.Refresh(0, CooldownMode.Normal);
            AdrenalineRush.Buff.Refresh(0, CooldownMode.Normal);
        }

        public DurationCooldownIndicator AdrenalineRush { get; set; }
        public DurationCooldownIndicator GuardianShout { get; set; }
        public Cooldown Infuriate { get; set; }
        public LancerLineHeldTracker LH { get; set; }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == GuardianShout.Cooldown.Skill.IconName)
            {
                GuardianShout.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == AdrenalineRush.Cooldown.Skill.IconName)
            {
                AdrenalineRush.Cooldown.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != Infuriate.Skill.IconName) return false;
            Infuriate.Start(sk.Duration);
            return true;
        }

        public override void LoadSpecialSkills()
        {
            Game.DB.SkillsDatabase.TryGetSkill(70300, Class.Lancer, out var gshout);
            Game.DB.SkillsDatabase.TryGetSkill(170200, Class.Lancer, out var arush);
            Game.DB.SkillsDatabase.TryGetSkill(120100, Class.Lancer, out var infu);

            GuardianShout = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(gshout, true) { CanFlash = true },
                Buff = new Cooldown(gshout, false)
            };
            AdrenalineRush = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(arush, true) { CanFlash = true },
                Buff = new Cooldown(arush, false)
            };

            Infuriate = new Cooldown(infu, true) { CanFlash = true };
        }

        public override void Dispose()
        {
            GuardianShout.Cooldown.Dispose();
            AdrenalineRush.Cooldown.Dispose();
            Infuriate.Dispose();
        }
    }
}