using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class ValkyrieLayoutVM : BaseClassLayoutVM
    {
        public Counter RunemarksCounter { get; set; }
        public SkillWithEffect Ragnarok { get; }
        public SkillWithEffect Godsfall { get; }

        public bool ShowRagnarok => App.Settings.ClassWindowSettings.ValkyrieShowRagnarok;
        public bool ShowGodsfall => App.Settings.ClassWindowSettings.ValkyrieShowGodsfall;

        public ValkyrieLayoutVM()
        {
            RunemarksCounter = new Counter(7, false);

            Game.DB.SkillsDatabase.TryGetSkill(120100, Class.Valkyrie, out var rag);
            Ragnarok = new SkillWithEffect(Dispatcher, rag);

            Game.DB.SkillsDatabase.TryGetSkill(250100, Class.Valkyrie, out var gf);
            Godsfall = new SkillWithEffect(Dispatcher, gf);
        }

        public override void Dispose()
        {
            Ragnarok.Dispose();
            Godsfall.Dispose();
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {

            if (sk.Skill.IconName == Ragnarok.Cooldown.Skill.IconName)
            {
                Ragnarok.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != Godsfall.Cooldown.Skill.IconName) return false;
            Godsfall.StartCooldown(sk.Duration);
            return true;
        }
    }
}