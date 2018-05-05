using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class BerserkerBarManager : ClassManager
    {

        public DurationCooldownIndicator FieryRage { get; set; }
        public DurationCooldownIndicator Bloodlust { get; set; }
        


        public BerserkerBarManager() : base()
        {
            //CurrentClassManager = this;
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(80600, Class.Berserker, out var fr);
            SessionManager.SkillsDatabase.TryGetSkill(210200, Class.Berserker, out var bl);
            FieryRage = new DurationCooldownIndicator(_dispatcher)
            {
                Cooldown = new FixedSkillCooldown(fr, _dispatcher, true),
                Buff = new FixedSkillCooldown(fr, _dispatcher, true)
            };
            Bloodlust = new DurationCooldownIndicator(_dispatcher)
            {
                Cooldown = new FixedSkillCooldown(bl, _dispatcher, true),
                Buff = new FixedSkillCooldown(bl, _dispatcher, true)
            };
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == FieryRage.Cooldown.Skill.IconName)
            {
                FieryRage.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Bloodlust.Cooldown.Skill.IconName)
            {
                Bloodlust.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
