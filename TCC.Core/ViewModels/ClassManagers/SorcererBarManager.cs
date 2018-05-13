using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class SorcererBarManager : ClassManager
    {
        public DurationCooldownIndicator ManaBoost { get; set; }

        public SorcererBarManager() : base()
        {

        }
        public override void LoadSpecialSkills()
        {
            ManaBoost = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(340200, Class.Sorcerer, out var mb);
            ManaBoost.Cooldown = new FixedSkillCooldown(mb, _dispatcher, true);
            ManaBoost.Buff = new FixedSkillCooldown(mb, _dispatcher, false);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == ManaBoost.Cooldown.Skill.IconName)
            {
                ManaBoost.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
