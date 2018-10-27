using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public class SorcererBarManager : ClassManager
    {
        public DurationCooldownIndicator ManaBoost { get; set; }

        public bool Fire => SessionManager.CurrentPlayer.Fire;
        public bool Ice => SessionManager.CurrentPlayer.Ice;
        public bool Arcane => SessionManager.CurrentPlayer.Arcane;

        public bool IsBoostFire => SessionManager.CurrentPlayer.FireBoost;
        public bool IsBoostFrost => SessionManager.CurrentPlayer.IceBoost;
        public bool IsBoostArcane => SessionManager.CurrentPlayer.ArcaneBoost;

        public override void LoadSpecialSkills()
        {
            ManaBoost = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(340200, Class.Sorcerer, out var mb);
            ManaBoost.Cooldown = new Cooldown(mb,  true);
            ManaBoost.Buff = new Cooldown(mb, false);
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == ManaBoost.Cooldown.Skill.IconName)
            {
                ManaBoost.Cooldown.Start(sk.Duration);
                return true;
            }
            return false;
        }

        public void NotifyElementChanged()
        {
            NPC(nameof(Fire));
            NPC(nameof(Ice));
            NPC(nameof(Arcane));
        }

        public void NotifyElementBoostChanged()
        {
            NPC(nameof(IsBoostFire));
            NPC(nameof(IsBoostFrost));
            NPC(nameof(IsBoostArcane));
        }
    }
}
