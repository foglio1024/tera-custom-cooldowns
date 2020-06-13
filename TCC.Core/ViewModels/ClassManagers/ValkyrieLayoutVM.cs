using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class ValkyrieLayoutVM : BaseClassLayoutVM
    {
        public Counter RunemarksCounter { get; set; }
        public DurationCooldownIndicator Ragnarok { get; private set; }
        public DurationCooldownIndicator Godsfall { get; private set; }

        public bool ShowRagnarok => App.Settings.ClassWindowSettings.ValkyrieShowRagnarok;
        public bool ShowGodsfall => App.Settings.ClassWindowSettings.ValkyrieShowGodsfall;

        public ValkyrieLayoutVM()
        {
            RunemarksCounter = new Counter(7, false);
        }

        public override void LoadSpecialSkills()
        {
            //Ragnarok
            Game.DB.SkillsDatabase.TryGetSkill(120100, Class.Valkyrie, out var rag);
            Game.DB.SkillsDatabase.TryGetSkill(250100, Class.Valkyrie, out var gf);
            Ragnarok = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(rag, true) { CanFlash = true },
                Buff = new Cooldown(rag, false)
            };
            Godsfall = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(gf, true) { CanFlash = true },
                Buff = new Cooldown(gf, false)
            };
        }

        public override void Dispose()
        {
            Ragnarok.Cooldown.Dispose();
            Godsfall.Cooldown.Dispose();
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {

            if (sk.Skill.IconName == Ragnarok.Cooldown.Skill.IconName)
            {
                Ragnarok.Cooldown.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != Godsfall.Cooldown.Skill.IconName) return false;
            Godsfall.Cooldown.Start(sk.Duration);
            return true;
        }
    }
}