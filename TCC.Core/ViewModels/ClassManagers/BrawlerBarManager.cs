using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class BrawlerBarManager : ClassManager
    {
        private bool _isGfOn;
        private bool _counterProc;
        public BrawlerBarManager() : base()
        {
        }

        public FixedSkillCooldown GrowingFury { get; set; }
        public FixedSkillCooldown Counter { get; set; }

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

            SessionManager.SkillsDatabase.TryGetSkill(180100, Class.Brawler, out var gf);
            SessionManager.SkillsDatabase.TryGetSkill(21200, Class.Brawler, out var c);
            GrowingFury = new FixedSkillCooldown(gf,  false);
            Counter = new FixedSkillCooldown(c,  false);
        }

    }
}
