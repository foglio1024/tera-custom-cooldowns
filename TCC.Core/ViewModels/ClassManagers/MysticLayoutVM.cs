using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels
{
    internal class MysticLayoutVM : BaseClassLayoutVM
    {
        private bool _elementalize;
        public AurasTracker Auras { get; }
        public Cooldown Contagion { get; }
        public SkillWithEffect Vow { get; }
        public SkillWithEffect VolleyOfCurse { get; }
        public Cooldown ThrallOfProtection { get; }
        public SkillWithEffect ThrallOfVengeance { get; }
        public SkillWithEffect ThrallOfWrath { get; }
        public Cooldown ThrallOfLife { get; }

        public Cooldown AuraMerciless { get; }
        public Cooldown AuraTenacious { get; }
        public Cooldown AuraSwift { get; }
        public Cooldown AuraUnyielding { get; }
        public string ElementalizeIcon { get; } = "icon_skills.spiritedness_tex";
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
        public bool ElementalizeWarning => !Elementalize && (Game.Combat || Game.Encounter);
        public bool OffenseAuraWarning => !Auras.OffenseAura && (Game.Combat || Game.Encounter);
        public bool SupportAuraWarning => !Auras.SupportAura && (Game.Combat || Game.Encounter);

        public MysticLayoutVM()
        {
            Auras = new AurasTracker();
            Game.DB.SkillsDatabase.TryGetSkill(251900, Class.Mystic, out var top);
            ThrallOfProtection = new Cooldown(top, false);
            
            Game.DB.SkillsDatabase.TryGetSkill(271600, Class.Mystic, out var tol);
            ThrallOfLife = new Cooldown(tol, false);
            
            Game.DB.SkillsDatabase.TryGetSkill(331700, Class.Mystic, out var tov);
            ThrallOfVengeance = new SkillWithEffect(Dispatcher, tov);
            
            Game.DB.SkillsDatabase.TryGetSkill(341600, Class.Mystic, out var tow);
            ThrallOfWrath = new SkillWithEffect(Dispatcher, tow);


            Game.DB.SkillsDatabase.TryGetSkill(160500, Class.Mystic, out var at);
            AuraTenacious = new Cooldown(at, false) { CanFlash = true };
            
            Game.DB.SkillsDatabase.TryGetSkill(130500, Class.Mystic, out var am);
            AuraMerciless = new Cooldown(am, false) { CanFlash = true };
            
            Game.DB.SkillsDatabase.TryGetSkill(140500, Class.Mystic, out var asw);
            AuraSwift = new Cooldown(asw, false) { CanFlash = true };
            
            Game.DB.SkillsDatabase.TryGetSkill(150600, Class.Mystic, out var au);
            AuraUnyielding = new Cooldown(au, false) { CanFlash = true };


            Game.DB.SkillsDatabase.TryGetSkill(120100, Class.Mystic, out var vow);
            Vow = new SkillWithEffect(Dispatcher, vow);
            Vow.Cooldown.FlashOnAvailable = false;
            Vow.Effect.Ended += OnVowBuffEnded;
            Vow.Effect.Started += OnVowBuffStarted;

            Game.DB.SkillsDatabase.TryGetSkill(241210, Class.Mystic, out var voc);
            VolleyOfCurse = new SkillWithEffect(Dispatcher, voc);

            Game.DB.SkillsDatabase.TryGetSkill(410100, Class.Mystic, out var cont);
            Contagion = new Cooldown(cont, true) { CanFlash = true };


            AbnormalityTracker.MarkingExpired += OnVocExpired;
            AbnormalityTracker.MarkingRefreshed += OnVocRefreshed;

            Game.CombatChanged += OnCombatChanged;
            Game.EncounterChanged += OnCombatChanged;
            Auras.AuraChanged += CheckAurasWarning;
        }

        public override void Dispose()
        {
            ThrallOfVengeance.Dispose();
            ThrallOfWrath.Dispose();

            AuraTenacious.Dispose();
            AuraMerciless.Dispose();
            AuraSwift.Dispose();
            AuraUnyielding.Dispose();

            Contagion.Dispose();

            Vow.Dispose();
            VolleyOfCurse.Dispose();
        }

        private void CheckAurasWarning()
        {
            AuraMerciless.FlashOnAvailable = !Auras.CritAura && !Auras.SwiftAura;
            AuraSwift.FlashOnAvailable = !Auras.CritAura && !Auras.SwiftAura;
            AuraTenacious.FlashOnAvailable = !Auras.ManaAura && !Auras.CritResAura;
            AuraUnyielding.FlashOnAvailable = !Auras.ManaAura && !Auras.CritResAura;

            N(nameof(OffenseAuraWarning));
            N(nameof(SupportAuraWarning));
        }
        private void OnCombatChanged()
        {
            N(nameof(ElementalizeWarning));
            N(nameof(OffenseAuraWarning));
            N(nameof(SupportAuraWarning));
            CheckAurasWarning();
        }

        private void OnVowBuffStarted(ulong cd, CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;
        private void OnVowBuffEnded(CooldownMode obj) => Vow.Cooldown.FlashOnAvailable = true;
        private void OnVocRefreshed(ulong duration)
        {
            VolleyOfCurse.RefreshEffect(duration);
            VolleyOfCurse.Cooldown.FlashOnAvailable = false;
        }

        private void OnVocExpired()
        {
            VolleyOfCurse.StopEffect();
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
                VolleyOfCurse.StartCooldown(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Vow.Cooldown.Skill.IconName)
            {
                Vow.StartCooldown(sk.Duration);
                return true;
            }
            //if (sk.Skill.IconName == ThrallOfProtection.Skill.IconName)
            //{
            //    ThrallOfProtection.Start(sk.Cooldown);
            //    return true;
            //}
            if (sk.Skill.IconName == ThrallOfVengeance.Cooldown.Skill.IconName)
            {
                ThrallOfVengeance.StartCooldown(sk.Duration);
                return true;
            }
            //if (sk.Skill.IconName == ThrallOfLife.Skill.IconName)
            //{
            //    ThrallOfLife.Start(sk.Cooldown);
            //    return true;
            //}
            if (sk.Skill.IconName != ThrallOfWrath.Cooldown.Skill.IconName) return false;
            ThrallOfWrath.StartCooldown(sk.Duration);
            return true;
            //if (sk.Skill.IconName == KingBlob.Skill.IconName)
            //{
            //    KingBlob.Start(sk.Cooldown);
            //    return true;
            //}
        }
    }
}