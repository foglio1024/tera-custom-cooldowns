using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public class ArcherLayoutVM : BaseClassLayoutVM
    {
        private bool _windWalkProc;

        private ArcherFocusTracker _focus;
        //private StanceTracker<ArcherStance> _stance;
        private Cooldown _thunderbolt;
        private DurationCooldownIndicator _windsong;
        private Cooldown _windWalk;

        public ArcherFocusTracker Focus
        {
            get => _focus;
            private set
            {
                if (_focus == value) return;
                _focus = value;
                N();
            }
        }
        //public StanceTracker<ArcherStance> Stance
        //{
        //    get => _stance;
        //    set
        //    {
        //        if(_stance== value) return;
        //        _stance = value;
        //        NPC();
        //    }
        //}
        public Cooldown Thunderbolt
        {
            get => _thunderbolt;
            set
            {
                if (_thunderbolt == value) return;
                _thunderbolt = value;
                N();
            }
        }
        public DurationCooldownIndicator Windsong
        {
            get => _windsong;
            set
            {
                if (_windsong == value) return;
                _windsong = value;
                N();
            }
        }
        public Cooldown WindWalk
        {
            get => _windWalk;
            set
            {
                if (_windWalk == value) return;
                _windWalk = value;
                N();
            }
        }
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
            //Stance = new StanceTracker<ArcherStance>();
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(290100, Class.Archer, out var tb);    // Thunderbolt
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(350100, Class.Archer, out var ws);    // Windsong
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(340100, Class.Archer, out var ww);    // Wind Walk
            Thunderbolt = new Cooldown(tb, true) { CanFlash = true };
            Windsong = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(ws, true) { CanFlash = true },
                Buff = new Cooldown(ws, false)
            };
            WindWalk = new Cooldown(ww, false);
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == Thunderbolt.Skill.IconName)
            {
                Thunderbolt.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Windsong.Cooldown.Skill.IconName)
            {
                Windsong.Cooldown.Start(sk.Duration);
                return true;
            }
            return false;
        }

        public override bool ResetSpecialSkill(Skill skill)
        {
            if (skill.IconName == Thunderbolt.Skill.IconName)
            {
                Thunderbolt.Refresh(0, CooldownMode.Normal);
                return true;
            }
            return false;
        }

        public override void Dispose()
        {
            Windsong.Cooldown.Dispose();
            Thunderbolt.Dispose();
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)       // KR patch by HQ
        {
            if (skill.IconName == Thunderbolt.Skill.IconName)
            {
                Thunderbolt.Refresh(Thunderbolt.Skill.Id, cd, CooldownMode.Normal);
                return true;
            }
            return false;
        }
    }
}
