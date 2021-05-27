using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers
{
    public class ArcherLayoutVM : BaseClassLayoutVM
    {
        private bool _windWalkProc;

        public ArcherFocusTracker Focus { get; private set; }
        public Cooldown Thunderbolt { get; set; }
        public SkillWithEffect Windsong { get; set; }
        public Cooldown WindWalk { get; set; }

        public bool WindWalkProc
        {
            get => _windWalkProc;
            set
            {
                if (_windWalkProc == value) return;
                _windWalkProc = value;
                N(nameof(WindWalkProc));
            }
        }

        public ArcherLayoutVM()
        {
            Focus = new ArcherFocusTracker();
            Game.DB!.SkillsDatabase.TryGetSkill(290100, Class.Archer, out var tb);    // Thunderbolt
            Game.DB!.SkillsDatabase.TryGetSkill(350100, Class.Archer, out var ws);    // Windsong
            Game.DB!.SkillsDatabase.TryGetSkill(340100, Class.Archer, out var ww);    // Wind Walk
            Thunderbolt = new Cooldown(tb, true) { CanFlash = true };
            Windsong = new SkillWithEffect(Dispatcher, ws);
            WindWalk = new Cooldown(ww, false);
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == Thunderbolt.Skill.IconName)
            {
                Thunderbolt.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != Windsong.Cooldown.Skill.IconName) return false;

            Windsong.StartCooldown(sk.Duration);
            return true;
        }

        public override bool ResetSpecialSkill(Skill skill)
        {
            if (skill.IconName != Thunderbolt.Skill.IconName) return false;
            Thunderbolt.Stop();
            return true;
        }

        public override void Dispose()
        {
            Windsong.Dispose();
            Thunderbolt.Dispose();
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)       // KR patch by HQ
        {
            if (skill.IconName != Thunderbolt.Skill.IconName) return false;
            Thunderbolt.Refresh(Thunderbolt.Skill.Id, cd, CooldownMode.Normal);
            return true;
        }
    }
}
