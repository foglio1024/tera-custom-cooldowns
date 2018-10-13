using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{
    public class ValkyrieBarManager : ClassManager
    {
        public Counter RunemarksCounter { get; set; }
        public DurationCooldownIndicator Ragnarok { get; private set; }

        public ValkyrieBarManager()
        {
            RunemarksCounter = new Counter(7, false);
        }

        public override void LoadSpecialSkills()
        {
            //Ragnarok
            Ragnarok = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Valkyrie, out var rag);
            Ragnarok.Cooldown = new FixedSkillCooldown(rag, true);
            Ragnarok.Buff = new FixedSkillCooldown(rag, false);
        }
        public override bool StartSpecialSkill(SkillCooldown sk)
        {

            if (sk.Skill.IconName == Ragnarok.Cooldown.Skill.IconName)
            {
                Ragnarok.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}