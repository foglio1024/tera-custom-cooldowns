using TCC.ClassSpecific;
using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    internal class MysticLayoutVM : BaseClassLayoutVM
    {
        private bool _elementalize;
        public AurasTracker Auras { get; private set; }
        public Cooldown Contagion { get; private set; }
        public DurationCooldownIndicator Vow { get; private set; }
        public DurationCooldownIndicator VolleyOfCurse { get; private set; }
        public Cooldown ThrallOfProtection { get; private set; }
        public Cooldown ThrallOfVengeance { get; private set; }
        public Cooldown ThrallOfWrath { get; private set; }
        public Cooldown ThrallOfLife { get; private set; }
        public Cooldown KingBlob { get; private set; }

        public Cooldown AuraMerciless { get; private set; }
        public Cooldown AuraTenacious { get; private set; }
        public Cooldown AuraSwift { get; private set; }
        public Cooldown AuraUnyielding { get; private set; }
        public bool Elementalize
        {
            get => _elementalize;
            set
            {
                if (_elementalize == value) return;
                _elementalize = value;
                N();
                N(nameof(ElementalizeWarning));
            }
        }
        public bool ElementalizeWarning => !Elementalize && (SessionManager.Combat || SessionManager.Encounter);

        public MysticLayoutVM()
        {
            Auras = new AurasTracker();
        }


        public override void LoadSpecialSkills()
        {
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(410100, Class.Mystic, out var cont);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(120100, Class.Mystic, out var vow);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(241210, Class.Mystic, out var voc);

            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(251900, Class.Mystic, out var top);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(331700, Class.Mystic, out var tov);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(341600, Class.Mystic, out var tow);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(271600, Class.Mystic, out var tol);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(480100, Class.Mystic, out var kb);

            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(130500, Class.Mystic, out var am);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(160500, Class.Mystic, out var at);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(140500, Class.Mystic, out var asw);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(150600, Class.Mystic, out var au);

            ThrallOfProtection = new Cooldown(top, false);
            ThrallOfLife = new Cooldown(tol, false);
            ThrallOfVengeance = new Cooldown(tov, true) { CanFlash = true };
            ThrallOfWrath = new Cooldown(tow, true) { CanFlash = true };
            KingBlob = new Cooldown(kb, true) { CanFlash = true };

            AuraTenacious = new Cooldown(at, false) { CanFlash = true };
            AuraMerciless = new Cooldown(am, false) { CanFlash = true };
            AuraSwift = new Cooldown(asw, false) { CanFlash = true };
            AuraUnyielding = new Cooldown(au, false) { CanFlash = true };

            Contagion = new Cooldown(cont, true) { CanFlash = true };

            Vow = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(vow, false),
                Cooldown = new Cooldown(vow, false) { CanFlash = true }
            };
            Vow.Buff.Ended += OnVowBuffEnded;
            Vow.Buff.Started += OnVowBuffStarted;

            VolleyOfCurse = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(voc, false),
                Cooldown = new Cooldown(voc, false) { CanFlash = true }
            };

            ClassAbnormalityTracker.MarkingExpired += OnVocExpired;
            ClassAbnormalityTracker.MarkingRefreshed += OnVocRefreshed;

            SessionManager.CombatChanged += OnCombatChanged;
            SessionManager.EncounterChanged += OnCombatChanged;
            Auras.AuraChanged += CheckAurasWarning;
        }

        public override void Dispose()
        {
            ThrallOfVengeance.Dispose();
            ThrallOfWrath.Dispose();
            KingBlob.Dispose();

            AuraTenacious.Dispose();
            AuraMerciless.Dispose();
            AuraSwift.Dispose();
            AuraUnyielding.Dispose();

            Contagion.Dispose();

            Vow.Cooldown.Dispose();
            VolleyOfCurse.Cooldown.Dispose();
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
            N(nameof(ElementalizeWarning));
            CheckAurasWarning();
        }

        private void OnVowBuffStarted(CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;
        private void OnVowBuffEnded(CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;
        private void OnVocRefreshed(ulong duration)
        {
            VolleyOfCurse.Buff.Refresh(duration, CooldownMode.Normal);
            VolleyOfCurse.Cooldown.FlashOnAvailable = false;
        }

        private void OnVocExpired()
        {
            VolleyOfCurse.Buff.Refresh(0, CooldownMode.Normal);
            VolleyOfCurse.Cooldown.FlashOnAvailable = true;
        }


        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == Contagion.Skill.IconName)
            {
                Contagion.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == VolleyOfCurse.Cooldown.Skill.IconName)
            {
                VolleyOfCurse.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Vow.Cooldown.Skill.IconName)
            {
                Vow.Cooldown.Start(sk.Duration);
                return true;
            }
            //if (sk.Skill.IconName == ThrallOfProtection.Skill.IconName)
            //{
            //    ThrallOfProtection.Start(sk.Cooldown);
            //    return true;
            //}
            if (sk.Skill.IconName == ThrallOfVengeance.Skill.IconName)
            {
                ThrallOfVengeance.Start(sk.Duration);
                return true;
            }
            //if (sk.Skill.IconName == ThrallOfLife.Skill.IconName)
            //{
            //    ThrallOfLife.Start(sk.Cooldown);
            //    return true;
            //}
            if (sk.Skill.IconName == ThrallOfWrath.Skill.IconName)
            {
                ThrallOfWrath.Start(sk.Duration);
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