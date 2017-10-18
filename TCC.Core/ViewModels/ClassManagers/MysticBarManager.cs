using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    internal class MysticBarManager : ClassManager
    {
        private static MysticBarManager _instance;
        public static MysticBarManager Instance => _instance ?? (_instance = new MysticBarManager());

        public AurasTracker Auras { get; private set; }
        public FixedSkillCooldown Contagion { get; private set; }
        public DurationCooldownIndicator Vow { get; private set; }

        public MysticBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
            Auras = new AurasTracker();

            LoadSpecialSkills();
            Vow.Buff.PropertyChanged += Vow_PropertyChanged;
        }

        private void Vow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Vow.Buff.IsAvailable))
            {
                Vow.Cooldown.FlashOnAvailable = Vow.Buff.IsAvailable;
            }
        }

        protected override void LoadSpecialSkills()
        {
            SkillsDatabase.TryGetSkill(410100, Class.Elementalist, out Skill cont);
            SkillsDatabase.TryGetSkill(120100, Class.Elementalist, out Skill vow);
            Contagion = new FixedSkillCooldown(cont, CooldownType.Skill, _dispatcher, true);
            Vow = new DurationCooldownIndicator(_dispatcher);
            Vow.Buff = new FixedSkillCooldown(vow, CooldownType.Skill, _dispatcher, false);
            Vow.Cooldown = new FixedSkillCooldown(vow, CooldownType.Skill,_dispatcher,false);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if(sk.Skill.IconName == Contagion.Skill.IconName)
            {
                Contagion.Start(sk.Cooldown);
                return true;
            }
            
            if(sk.Skill.IconName == Vow.Cooldown.Skill.IconName)
            {
                Vow.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}