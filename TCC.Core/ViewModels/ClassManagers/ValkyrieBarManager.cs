using TCC.Data;

namespace TCC.ViewModels
{
    public class ValkyrieBarManager : ClassManager
    {
        public Counter RunemarksCounter { get; set; }
        public DurationCooldownIndicator Ragnarok { get; private set; }
        public DurationCooldownIndicator Godsfall { get; private set; }

        public ValkyrieBarManager()
        {
            RunemarksCounter = new Counter(7, false);
        }

        public override void LoadSpecialSkills()
        {
            //Ragnarok
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Valkyrie, out var rag);
            SessionManager.SkillsDatabase.TryGetSkill(250100, Class.Valkyrie, out var gf);
            Ragnarok = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(rag, true),
                Buff = new Cooldown(rag, false)
            };
            Godsfall = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(gf, true),
                Buff = new Cooldown(gf, false)
            };
        }
        public override bool StartSpecialSkill(Cooldown sk)
        {

            if (sk.Skill.IconName == Ragnarok.Cooldown.Skill.IconName)
            {
                Ragnarok.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Godsfall.Cooldown.Skill.IconName)
            {
                Godsfall.Cooldown.Start(sk.Duration);
                return true;
            }
            return false;
        }
    }
}