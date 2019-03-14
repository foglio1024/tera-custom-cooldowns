using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public class BerserkerLayoutVM : BaseClassLayoutVM
    {

        private bool _isUnleashOn;
        private bool _isUnleashOff = true;

        public DurationCooldownIndicator FieryRage { get; set; }
        public DurationCooldownIndicator Bloodlust { get; set; }
        public DurationCooldownIndicator Unleash { get; set; }

        public Cooldown Dexter { get; set; }
        public Cooldown Sinister { get; set; }
        public Cooldown Rampage { get; set; }
        public Cooldown BeastFury { get; set; }

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
                N(nameof(IsUnleashOn));
            }
        }

        public bool IsUnleashOff
        {
            get => _isUnleashOff;
            set
            {
                if (_isUnleashOff == value) return;
                _isUnleashOff = value;
                N(nameof(IsUnleashOff));
            }
        }

        public BerserkerLayoutVM()
        {
            SinisterTracker = new StatTracker();
            DexterTracker = new StatTracker();
            RampageTracker = new StatTracker();
        }

        public override void LoadSpecialSkills()
        {
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(80600, Class.Berserker, out var fr);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(210200, Class.Berserker, out var bl);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(330100, Class.Berserker, out var ul);
            FieryRage = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(fr,  true) { CanFlash = true },
                Buff = new Cooldown(fr,  true)
            };
            Bloodlust = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(bl,  true) { CanFlash = true },
                Buff = new Cooldown(bl,  true)
            };
            Unleash = new DurationCooldownIndicator(Dispatcher)
            {
                Cooldown = new Cooldown(ul, false) { CanFlash = true },
                Buff = new Cooldown(ul, false)
            };

            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(340100, Class.Berserker, out var dx);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(350100, Class.Berserker, out var sx);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(360100, Class.Berserker, out var rp);
            SessionManager.CurrentDatabase.SkillsDatabase.TryGetSkill(370100, Class.Berserker, out var bf);

            Dexter = new Cooldown(dx, false);
            Sinister = new Cooldown(sx, false);
            Rampage = new Cooldown(rp, false);
            BeastFury = new Cooldown(bf, false);
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == FieryRage.Cooldown.Skill.IconName)
            {
                FieryRage.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Bloodlust.Cooldown.Skill.IconName)
            {
                Bloodlust.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == Unleash.Cooldown.Skill.IconName)
            {
                Unleash.Cooldown.Start(sk.Duration);
                return true;
            }
            if (sk.Skill.IconName == BeastFury.Skill.IconName)
            {
                BeastFury.Start(sk.Duration);
                return true;
            }
            return false;
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            if (skill.IconName == Unleash.Cooldown.Skill.IconName)
            {
                Unleash.Cooldown.Refresh(skill.Id, cd, CooldownMode.Normal);
                return true;
            }
            return false;
        }

        public override void Dispose()
        {
            FieryRage.Cooldown.Dispose();
            Bloodlust.Cooldown.Dispose();
            Unleash.Cooldown.Dispose();
        }
    }
}
