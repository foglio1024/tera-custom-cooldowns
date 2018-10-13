using TCC.Data;

namespace TCC.ViewModels
{
    public class BerserkerBarManager : ClassManager
    {

        private bool _isUnleashOn;
        private bool _isUnleashOff = true;

        public DurationCooldownIndicator FieryRage { get; set; }
        public DurationCooldownIndicator Bloodlust { get; set; }
        public DurationCooldownIndicator Unleash { get; set; }

        public FixedSkillCooldown Dexter { get; set; }
        public FixedSkillCooldown Sinister { get; set; }
        public FixedSkillCooldown Rampage { get; set; }
        public FixedSkillCooldown BeastFury { get; set; }

        public StatTracker SinisterTracker { get; set; }
        public StatTracker DexterTracker { get; set; }
        public StatTracker RampageTracker { get; set; }

        public bool IsUnleashOn
        {
            get => _isUnleashOn;
            set
            {
                if (_isUnleashOn == value) return;
                _isUnleashOn = value;
                NPC(nameof(IsUnleashOn));
            }
        }

        public bool IsUnleashOff
        {
            get => _isUnleashOff;
            set
            {
                if (_isUnleashOff == value) return;
                _isUnleashOff = value;
                NPC(nameof(IsUnleashOff));
            }
        }

        public BerserkerBarManager()
        {
            SinisterTracker = new StatTracker();
            DexterTracker = new StatTracker();
            RampageTracker = new StatTracker();
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.SkillsDatabase.TryGetSkill(80600, Class.Berserker, out var fr);
            SessionManager.SkillsDatabase.TryGetSkill(210200, Class.Berserker, out var bl);
            SessionManager.SkillsDatabase.TryGetSkill(330100, Class.Berserker, out var ul);
            FieryRage = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new FixedSkillCooldown(fr,  true),
                Buff = new FixedSkillCooldown(fr,  true)
            };
            Bloodlust = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new FixedSkillCooldown(bl,  true),
                Buff = new FixedSkillCooldown(bl,  true)
            };
            Unleash = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new FixedSkillCooldown(ul, false),
                Buff = new FixedSkillCooldown(ul, false)
            };

            SessionManager.SkillsDatabase.TryGetSkill(340100, Class.Berserker, out var dx);
            SessionManager.SkillsDatabase.TryGetSkill(350100, Class.Berserker, out var sx);
            SessionManager.SkillsDatabase.TryGetSkill(360100, Class.Berserker, out var rp);
            SessionManager.SkillsDatabase.TryGetSkill(370100, Class.Berserker, out var bf);

            Dexter = new FixedSkillCooldown(dx, false);
            Sinister = new FixedSkillCooldown(sx, false);
            Rampage = new FixedSkillCooldown(rp, false);
            BeastFury = new FixedSkillCooldown(bf, false);
        }

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == FieryRage.Cooldown.Skill.IconName)
            {
                FieryRage.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Bloodlust.Cooldown.Skill.IconName)
            {
                Bloodlust.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Unleash.Cooldown.Skill.IconName)
            {
                Unleash.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == BeastFury.Skill.IconName)
            {
                BeastFury.Start(sk.Cooldown);
                return true;
            }
            return false;
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            if (skill.IconName == Unleash.Cooldown.Skill.IconName)
            {
                Unleash.Cooldown.Refresh(skill.Id, cd);
                return true;
            }
            return false;
        }
    }
}
