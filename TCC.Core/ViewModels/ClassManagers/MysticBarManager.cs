using TCC.ClassSpecific;
using TCC.Data;

namespace TCC.ViewModels
{
    internal class MysticBarManager : ClassManager
    {
        private bool _elementalize;
        public AurasTracker Auras { get; private set; }
        public FixedSkillCooldown Contagion { get; private set; }
        public DurationCooldownIndicator Vow { get; private set; }
        public DurationCooldownIndicator VolleyOfCurse { get; private set; }
        public FixedSkillCooldown ThrallOfProtection { get; private set; }
        public FixedSkillCooldown ThrallOfVengeance { get; private set; }
        public FixedSkillCooldown ThrallOfWrath { get; private set; }
        public FixedSkillCooldown ThrallOfLife { get; private set; }
        public FixedSkillCooldown KingBlob { get; private set; }

        public FixedSkillCooldown AuraMerciless { get; private set; }
        public FixedSkillCooldown AuraTenacious { get; private set; }
        public FixedSkillCooldown AuraSwift { get; private set; }
        public FixedSkillCooldown AuraUnyielding { get; private set; }
        public bool Elementalize
        {
            get => _elementalize;
            set
            {
                if (_elementalize == value) return;
                _elementalize = value;
                NPC();
                NPC(nameof(ElementalizeWarning));
            }
        }
        public bool ElementalizeWarning => !Elementalize && (SessionManager.Combat || SessionManager.Encounter);

        public MysticBarManager()
        {
            Auras = new AurasTracker();
        }


        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(410100, Class.Mystic, out var cont);
            SessionManager.SkillsDatabase.TryGetSkill(120100, Class.Mystic, out var vow);
            SessionManager.SkillsDatabase.TryGetSkill(241210, Class.Mystic, out var voc);

            SessionManager.SkillsDatabase.TryGetSkill(251900, Class.Mystic, out var top);
            SessionManager.SkillsDatabase.TryGetSkill(331700, Class.Mystic, out var tov);
            SessionManager.SkillsDatabase.TryGetSkill(341600, Class.Mystic, out var tow);
            SessionManager.SkillsDatabase.TryGetSkill(271600, Class.Mystic, out var tol);
            SessionManager.SkillsDatabase.TryGetSkill(480100, Class.Mystic, out var kb);

            SessionManager.SkillsDatabase.TryGetSkill(130500, Class.Mystic, out var am);
            SessionManager.SkillsDatabase.TryGetSkill(160500, Class.Mystic, out var at);
            SessionManager.SkillsDatabase.TryGetSkill(140500, Class.Mystic, out var asw);
            SessionManager.SkillsDatabase.TryGetSkill(150600, Class.Mystic, out var au);

            ThrallOfProtection = new FixedSkillCooldown(top, false);
            ThrallOfLife = new FixedSkillCooldown(tol, false);
            ThrallOfVengeance = new FixedSkillCooldown(tov, true);
            ThrallOfWrath = new FixedSkillCooldown(tow, true);
            KingBlob = new FixedSkillCooldown(kb, true);

            AuraTenacious = new FixedSkillCooldown(at, false);
            AuraMerciless = new FixedSkillCooldown(am, false);
            AuraSwift = new FixedSkillCooldown(asw, false);
            AuraUnyielding = new FixedSkillCooldown(au, false);

            Contagion = new FixedSkillCooldown(cont, true);

            Vow = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new FixedSkillCooldown(vow, false),
                Cooldown = new FixedSkillCooldown(vow, false)
            };
            Vow.Buff.Ended += OnVowBuffEnded;
            Vow.Buff.Started += OnVowBuffStarted;

            VolleyOfCurse = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new FixedSkillCooldown(voc, false),
                Cooldown = new FixedSkillCooldown(voc, false)
            };

            ClassAbnormalityTracker.MarkingExpired += OnVocExpired;
            ClassAbnormalityTracker.MarkingRefreshed += OnVocRefreshed;

            SessionManager.CombatChanged += OnCombatChanged;
            SessionManager.EncounterChanged += OnCombatChanged;
            Auras.AuraChanged += CheckAurasWarning;
        }

        private void CheckAurasWarning()
        {
            AuraMerciless.FlashOnAvailable = !Auras.CritAura && !Auras.SwiftAura;
            AuraSwift.FlashOnAvailable = !Auras.CritAura && !Auras.SwiftAura;
            AuraTenacious.FlashOnAvailable = !Auras.ManaAura && !Auras.CritResAura;
            AuraUnyielding.FlashOnAvailable = !Auras.ManaAura && !Auras.CritResAura;
        }
        private void OnCombatChanged()
        {
            NPC(nameof(ElementalizeWarning));
            CheckAurasWarning();
        }

        private void OnVowBuffStarted(CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;
        private void OnVowBuffEnded(CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;
        private void OnVocRefreshed(ulong duration)
        {
            VolleyOfCurse.Buff.Refresh(duration);
            VolleyOfCurse.Cooldown.FlashOnAvailable = false;
        }

        private void OnVocExpired()
        {
            VolleyOfCurse.Buff.Refresh(0);
            VolleyOfCurse.Cooldown.FlashOnAvailable = true;
        }


        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == Contagion.Skill.IconName)
            {
                Contagion.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == VolleyOfCurse.Cooldown.Skill.IconName)
            {
                VolleyOfCurse.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Vow.Cooldown.Skill.IconName)
            {
                Vow.Cooldown.Start(sk.Cooldown);
                return true;
            }
            //if (sk.Skill.IconName == ThrallOfProtection.Skill.IconName)
            //{
            //    ThrallOfProtection.Start(sk.Cooldown);
            //    return true;
            //}
            if (sk.Skill.IconName == ThrallOfVengeance.Skill.IconName)
            {
                ThrallOfVengeance.Start(sk.Cooldown);
                return true;
            }
            //if (sk.Skill.IconName == ThrallOfLife.Skill.IconName)
            //{
            //    ThrallOfLife.Start(sk.Cooldown);
            //    return true;
            //}
            if (sk.Skill.IconName == ThrallOfWrath.Skill.IconName)
            {
                ThrallOfWrath.Start(sk.Cooldown);
                return true;
            }
            //if (sk.Skill.IconName == KingBlob.Skill.IconName)
            //{
            //    KingBlob.Start(sk.Cooldown);
            //    return true;
            //}
            return false;
        }
    }
}