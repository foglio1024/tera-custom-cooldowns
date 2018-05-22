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


        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(410100, Class.Mystic, out var cont);
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Mystic, out var vow);
            Contagion = new FixedSkillCooldown(cont, true);
            Vow = new DurationCooldownIndicator(_dispatcher)
            {
                Buff = new FixedSkillCooldown(vow, false),
                Cooldown = new FixedSkillCooldown(vow, false)
            };
            Vow.Buff.Ended += OnVowBuffEnded;
            Vow.Buff.Started += OnVowBuffStarted;
        }

        private void OnVowBuffStarted(CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;
        private void OnVowBuffEnded(CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;


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