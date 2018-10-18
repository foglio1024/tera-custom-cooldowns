using TCC.Data;

namespace TCC.ViewModels
{
    public class SorcererBarManager : ClassManager
    {


        public DurationCooldownIndicator ManaBoost { get; set; }

        public bool Fire => SessionManager.CurrentPlayer.Fire;
        public bool Ice => SessionManager.CurrentPlayer.Ice;
        public bool Arcane => SessionManager.CurrentPlayer.Arcane;

        public override void LoadSpecialSkills()
        {
            ManaBoost = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(340200, Class.Sorcerer, out var mb);
            ManaBoost.Cooldown = new FixedSkillCooldown(mb,  true);
            ManaBoost.Buff = new FixedSkillCooldown(mb, false);
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

        public void NotiftElementChanged()
        {
            NPC(nameof(Fire));
            NPC(nameof(Ice));
            NPC(nameof(Arcane));
        }
    }
}
