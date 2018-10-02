using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{
    public class BrawlerBarManager : ClassManager
    {
        private bool _isGfOn;
        private bool _counterProc;
        public BrawlerBarManager()
        {
            AbnormalityTracker = new BrawlerAbnormalityTracker();
        }

        public DurationCooldownIndicator GrowingFury { get; set; }
        public FixedSkillCooldown Counter { get; set; }
        public DurationCooldownIndicator RhythmicBlows { get; set; }
        public DurationCooldownIndicator Infuriate { get; set; }


        public bool IsGfOn
        {
            get => _isGfOn;
            set
            {
                if (_isGfOn == value) return;
                _isGfOn = value;
                NPC(nameof(IsGfOn));
            }
        }

        public bool CounterProc
        {
            get => _counterProc;
            set
            {
                if (_counterProc == value) return;
                _counterProc = value;
                NPC(nameof(CounterProc));
            }
        }

        public override void LoadSpecialSkills()
        {
            // Growing Fury
            GrowingFury = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(180100, Class.Brawler, out var gf);
            GrowingFury.Cooldown = new FixedSkillCooldown(gf,  true);
            GrowingFury.Buff = new FixedSkillCooldown(gf, false);

            // Counter 
            //Counter = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(21200, Class.Brawler, out var c);
            Counter = new FixedSkillCooldown(c, false);

            // Rhythmic Blows
            RhythmicBlows = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(260100, Class.Brawler, out var rb);
            RhythmicBlows.Cooldown = new FixedSkillCooldown(rb, true);

            // Infuriate
            Infuriate = new DurationCooldownIndicator(Dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(140100, Class.Brawler, out var infu);
            Infuriate.Cooldown = new FixedSkillCooldown(infu, true);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == RhythmicBlows.Cooldown.Skill.IconName)
            {
                RhythmicBlows.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Infuriate.Cooldown.Skill.IconName)
            {
                Infuriate.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            if (skill.IconName == RhythmicBlows.Cooldown.Skill.IconName)
            {
                RhythmicBlows.Cooldown.Refresh(cd);
                return true;
            }
            return false;
        }
    }
}
