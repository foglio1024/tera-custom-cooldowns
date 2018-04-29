using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class NinjaBarManager : ClassManager
    {
        private static NinjaBarManager _instance;
        private bool _focusOn;
        public static NinjaBarManager Instance => _instance ?? (_instance = new NinjaBarManager());

        public FixedSkillCooldown BurningHeart { get; set; }
        public FixedSkillCooldown FireAvalanche { get; set; }

        public bool FocusOn
        {
            get { return _focusOn; }
            set
            {
                if (_focusOn == value) return;
                _focusOn = value;
                NPC(nameof(FocusOn));
            }

        }

        public NinjaBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSpecialSkills();
            ST.PropertyChanged += FlashOnMaxSt;
        }

        private void FlashOnMaxSt(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ST.Maxed))
            {
                BurningHeart.FlashOnAvailable = ST.Maxed;
                FireAvalanche.FlashOnAvailable = ST.Maxed;
            }
        }

        protected override void LoadSpecialSkills()
        {
            SkillsDatabase.TryGetSkill(150700, Class.Assassin, out var bh);
            SkillsDatabase.TryGetSkill(80200, Class.Assassin, out var fa);
            BurningHeart = new FixedSkillCooldown(bh, CooldownType.Skill, _dispatcher, false);
            FireAvalanche = new FixedSkillCooldown(fa, CooldownType.Skill, _dispatcher, false);

        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == FireAvalanche.Skill.IconName)
            {
                FireAvalanche.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == BurningHeart.Skill.IconName)
            {
                BurningHeart.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}
