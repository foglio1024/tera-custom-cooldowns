using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    internal class MysticBarManager : ClassManager
    {
        private bool _elementalize;
        public AurasTracker Auras { get; private set; }
        public FixedSkillCooldown Contagion { get; private set; }
        public DurationCooldownIndicator Vow { get; private set; }

        public bool Elementalize
        {
            get => _elementalize;
            set
            {
                if(_elementalize == value) return;
                _elementalize = value;
                NPC();
            }
        }

        public MysticBarManager() : base()
        {
            Auras = new AurasTracker();
        }

        private void Vow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Vow.Buff.IsAvailable))
            {
                Vow.Cooldown.FlashOnAvailable = Vow.Buff.IsAvailable;
            }
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(410100, Class.Elementalist, out var cont);
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Elementalist, out var vow);
            Contagion = new FixedSkillCooldown(cont, _dispatcher, true);
            Vow = new DurationCooldownIndicator(_dispatcher)
            {
                Buff = new FixedSkillCooldown(vow, _dispatcher, false),
                Cooldown = new FixedSkillCooldown(vow, _dispatcher, false)
            };
            Vow.Buff.PropertyChanged += Vow_PropertyChanged;


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