using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class BrawlerLayoutVM : BaseClassLayoutVM
    {
        private bool _isGfOn;
        private bool _counterProc;

        public SkillWithEffect GrowingFury { get;  }
        public Cooldown Counter { get;  }
        public Cooldown RhythmicBlows { get; }
        public Cooldown Infuriate { get;  }


        public bool IsGfOn
        {
            get => _isGfOn;
            set
            {
                if (_isGfOn == value) return;
                _isGfOn = value;
                N(nameof(IsGfOn));
            }
        }

        public bool CounterProc
        {
            get => _counterProc;
            set
            {
                if (_counterProc == value) return;
                _counterProc = value;
                N(nameof(CounterProc));
            }
        }


        public BrawlerLayoutVM()
        {
            // Growing Fury
            Game.DB.SkillsDatabase.TryGetSkill(180100, Class.Brawler, out var gf);
            GrowingFury = new SkillWithEffect(Dispatcher, gf);

            // Counter 
            Game.DB.SkillsDatabase.TryGetSkill(21200, Class.Brawler, out var c);
            Counter = new Cooldown(c, false);

            // Rhythmic Blows
            Game.DB.SkillsDatabase.TryGetSkill(260100, Class.Brawler, out var rb);
            RhythmicBlows = new Cooldown(rb, true) { CanFlash = true };

            // Infuriate
            Game.DB.SkillsDatabase.TryGetSkill(140100, Class.Brawler, out var infu);
            Infuriate = new Cooldown(infu, true) { CanFlash = true };

        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == RhythmicBlows.Skill.IconName)
            {
                RhythmicBlows.Start(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName != Infuriate.Skill.IconName) return false;
            Infuriate.Start(sk.Duration);
            return true;
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            if (skill.IconName == RhythmicBlows.Skill.IconName)
            {
                RhythmicBlows.Refresh(skill.Id, cd, CooldownMode.Normal);
                return true;
            }

            if (skill.IconName != Infuriate.Skill.IconName) return false;
            Infuriate.Refresh(cd, CooldownMode.Normal);
            return true;
        }

        public override void Dispose()
        {
            RhythmicBlows.Dispose();
            Infuriate.Dispose();
        }
    }
}
